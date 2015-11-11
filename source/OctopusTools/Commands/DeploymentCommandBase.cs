using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using log4net;
using Octopus.Client.Model;
using Octopus.Client.Model.Forms;
using OctopusTools.Infrastructure;
using OctopusTools.Util;
using Octostache;

namespace OctopusTools.Commands
{
    public abstract class DeploymentCommandBase : ApiCommand
    {
        readonly VariableDictionary variables = new VariableDictionary();

        protected DeploymentCommandBase(IOctopusRepositoryFactory repositoryFactory, ILog log) : base(repositoryFactory, log)
        {
            SpecificMachineNames = new List<string>();
            SkipStepNames = new List<string>();

            var options = Options.For("Deployment");
            options.Add("progress", "[Optional] Show progress of the deployment", v => { showProgress = true; WaitForDeployment = true; noRawLog = true; });
            options.Add("forcepackagedownload", "[Optional] Whether to force downloading of already installed packages (flag, default false).", v => ForcePackageDownload = true);
            options.Add("waitfordeployment", "[Optional] Whether to wait synchronously for deployment to finish.", v => WaitForDeployment = true);
            options.Add("deploymenttimeout=", "[Optional] Specifies maximum time (timespan format) that the console session will wait for the deployment to finish(default 00:10:00). This will not stop the deployment.", v => DeploymentTimeout = TimeSpan.Parse(v));
            options.Add("deploymentchecksleepcycle=", "[Optional] Specifies how much time (timespan format) should elapse between deployment status checks (default 00:00:10)", v => DeploymentStatusCheckSleepCycle = TimeSpan.Parse(v));
            options.Add("guidedfailure=", "[Optional] Whether to use Guided Failure mode. (True or False. If not specified, will use default setting from environment)", v => UseGuidedFailure = bool.Parse(v));
            options.Add("specificmachines=", "[Optional] A comma-separated list of machines names to target in the deployed environment. If not specified all machines in the environment will be considered.", v => SpecificMachineNames.AddRange(v.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(m => m.Trim())));
            options.Add("force", "[Optional] If a project is configured to skip packages with already-installed versions, override this setting to force re-deployment (flag, default false).", v => ForcePackageRedeployment = true);
            options.Add("skip=", "[Optional] Skip a step by name", v => SkipStepNames.Add(v));
            options.Add("norawlog", "[Optional] Don't print the raw log of failed tasks", v => noRawLog = true);
            options.Add("rawlogfile=", "[Optional] Redirect the raw log of failed tasks to a file", v => rawLogFile = v);
            options.Add("v|variable=", "[Optional] Values for any prompted variables in the format Label:Value", ParseVariable);
            options.Add("deployat=", "[Optional] Time at which deployment should start (scheduled deployment), specified as any valid DateTimeOffset format, and assuming the time zone is the current local time zone.", v => ParseDeployAt(v));
        }

        protected bool ForcePackageRedeployment { get; set; }
        protected bool ForcePackageDownload { get; set; }
        protected bool? UseGuidedFailure { get; set; }
        protected bool WaitForDeployment { get; set; }
        protected TimeSpan DeploymentTimeout { get; set; }
        protected TimeSpan DeploymentStatusCheckSleepCycle { get; set; }
        protected List<string> SpecificMachineNames { get; set; }
        protected List<string> SkipStepNames { get; set; }
        protected DateTimeOffset? DeployAt { get; set; }
        bool noRawLog;
        bool showProgress;
        string rawLogFile;
        TaskOutputProgressPrinter printer = new TaskOutputProgressPrinter();

        DateTimeOffset? ParseDeployAt(string v)
        {
            try
            {
                return DeployAt = DateTimeOffset.Parse(v, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal);
            }
            catch (FormatException fex)
            {
                throw new CommandException("Could not convert '" + v + "' to a DateTimeOffset: " + fex.Message);
            }
        }

        public void DeployRelease(ProjectResource project, ReleaseResource release, List<string> environments)
        {
            if (environments.Count == 0)
                return;

            var deployments = new List<DeploymentResource>();
            var deploymentTasks = new List<TaskResource>();

            var releaseTemplate = Repository.Releases.GetTemplate(release);

            var promotingEnvironments =
                (from environment in environments.Distinct(StringComparer.CurrentCultureIgnoreCase)
                    let promote = releaseTemplate.PromoteTo.FirstOrDefault(p => string.Equals(p.Name, environment))
                    select new {Name = environment, Promote = promote}).ToList();

            var unknownEnvironments = promotingEnvironments.Where(p => p.Promote == null).ToList();
            if (unknownEnvironments.Count > 0)
            {
                throw new CommandException(
                    string.Format("Release '{0}' of project '{1}' cannot be deployed to {2} not in the list of environments that this release can be deployed to. This may be because a) the environment does not exist, b) the name is misspelled, c) you don't have permission to deploy to this environment, or d) the environment is not in the list of environments defined by the lifecycle.",
                        release.Version,
                        project.Name,
                        unknownEnvironments.Count == 1
                            ? "environment '" + unknownEnvironments[0].Name + "' because the environment is"
                            : "environments " + string.Join(", ", unknownEnvironments.Select(e => "'" + e.Name + "'")) + " because the environments are"
                        ));
            }

            var specificMachineIds = new ReferenceCollection();
            if (SpecificMachineNames.Any())
            {
                var machines = Repository.Machines.FindByNames(SpecificMachineNames);
                var missing = SpecificMachineNames.Except(machines.Select(m => m.Name), StringComparer.OrdinalIgnoreCase).ToList();
                if (missing.Any())
                {
                    throw new CommandException("The following specific machines could not be found: " + missing.ReadableJoin());
                }

                specificMachineIds.AddRange(machines.Select(m => m.Id));
            }

            if (DeployAt != null)
            {
                var now = DateTimeOffset.UtcNow;
                Log.InfoFormat("Deployment will be scheduled to start in: {0}", (DeployAt.Value - now).FriendlyDuration());
            }

            foreach (var environment in promotingEnvironments)
            {
                var promote = environment.Promote;
                var preview = Repository.Releases.GetPreview(promote);

                var skip = new ReferenceCollection();
                foreach (var step in SkipStepNames)
                {
                    var stepToExecute = preview.StepsToExecute.SingleOrDefault(s => string.Equals(s.ActionName, step, StringComparison.CurrentCultureIgnoreCase));
                    if (stepToExecute == null)
                    {
                        Log.WarnFormat("No step/action named '{0}' could be found when deploying to environment '{1}', so the step cannot be skipped.", step, environment);
                    }
                    else
                    {
                        Log.DebugFormat("Skipping step: {0}", stepToExecute.ActionName);
                        skip.Add(stepToExecute.ActionId);
                    }
                }

                if (preview.Form != null && preview.Form.Elements != null && preview.Form.Values != null)
                {
                    foreach (var element in preview.Form.Elements)
                    {
                        var variableInput = element.Control as VariableValue;
                        if (variableInput == null)
                        {
                            continue;
                        }

                        var value = variables.Get(variableInput.Label) ?? variables.Get(variableInput.Name);

                        if (string.IsNullOrWhiteSpace(value) && element.IsValueRequired)
                        {
                            throw new ArgumentException("Please provide a variable for the prompted value " + variableInput.Label);
                        }

                        preview.Form.Values[element.Name] = value;
                    }
                }

                var deployment = Repository.Deployments.Create(new DeploymentResource
                {
                    EnvironmentId = promote.Id,
                    SkipActions = skip,
                    ReleaseId = release.Id,
                    ForcePackageDownload = ForcePackageDownload,
                    UseGuidedFailure = UseGuidedFailure.GetValueOrDefault(preview.UseGuidedFailureModeByDefault),
                    SpecificMachineIds = specificMachineIds,
                    ForcePackageRedeployment = ForcePackageRedeployment,
                    FormValues = (preview.Form ?? new Form()).Values,
                    QueueTime = DeployAt == null ? null : (DateTimeOffset?)DeployAt.Value
                });

                Log.InfoFormat("Deploying {0} {1} to: {2} (Guided Failure: {3})", project.Name, release.Version, environment.Name, deployment.UseGuidedFailure ? "Enabled" : "Not Enabled");

                foreach (var previewStep in preview.StepsToExecute)
                {
                    if (previewStep.HasNoApplicableMachines)
                    {
                        Log.Warn("Warning: there are no applicable machines roles used by step " + previewStep.ActionName + " step");
                    }
                }

                deployments.Add(deployment);
                deploymentTasks.Add(Repository.Tasks.Get(deployment.TaskId));
            }

            if (WaitForDeployment)
            {
                WaitForDeploymentToComplete(deploymentTasks, deployments, project, release);
            }
        }

        public void WaitForDeploymentToComplete(List<TaskResource> deploymentTasks, List<DeploymentResource> deployments, ProjectResource project, ReleaseResource release)
        {
            if (showProgress && deploymentTasks.Count > 1)
            {
                Log.InfoFormat("Only progress of the first task ({0}) will be shown", deploymentTasks.First().Description);
            }

            try
            {
                Log.InfoFormat("Waiting for {0} deployment(s) to complete....", deploymentTasks.Count);
                Repository.Tasks.WaitForCompletion(deploymentTasks.ToArray(), DeploymentStatusCheckSleepCycle.Seconds, (int)DeploymentTimeout.TotalMinutes, PrintTaskOutput);
                var failed = false;
                foreach (var deploymentTask in deploymentTasks)
                {
                    var updated = Repository.Tasks.Get(deploymentTask.Id);
                    if (updated.FinishedSuccessfully)
                    {
                        Log.InfoFormat("{0}: {1}", updated.Description, updated.State);
                    }
                    else
                    {
                        Log.ErrorFormat("{0}: {1}, {2}", updated.Description, updated.State, updated.ErrorMessage);
                        failed = true;

                        if (!noRawLog)
                        {
                            try
                            {
                                var raw = Repository.Tasks.GetRawOutputLog(updated);
                                if (!string.IsNullOrEmpty(rawLogFile))
                                {
                                    File.WriteAllText(rawLogFile, raw);
                                }
                                else
                                {
                                    Log.Error(raw);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Could not retrieve the raw task log for the failed task.", ex);
                            }
                        }
                    }
                }
                if (failed)
                {
                    throw new CommandException("One or more deployment tasks failed.");
                }

                Log.Info("Done!");
            }
            catch (TimeoutException e)
            {
                Log.Error(e.Message);
                var guidedFailureDeployments =
                    from d in deployments
                    where d.UseGuidedFailure
                    select d;
                if (guidedFailureDeployments.Any())
                {
                    Log.Warn("One or more deployments are using Guided Failure. Use the links below to check if intervention is required:");
                    foreach (var guidedFailureDeployment in guidedFailureDeployments)
                    {
                        var environment = Repository.Environments.Get(guidedFailureDeployment.Link("Environment"));
                        Log.WarnFormat("  - {0}: {1}", environment.Name, GetPortalUrl(string.Format("/app#/projects/{0}/releases/{1}/deployments/{2}", project.Slug, release.Version, guidedFailureDeployment.Id)));
                    }
                }
                throw new CommandException(e.Message);
            }
        }

        void PrintTaskOutput(TaskResource[] taskResources)
        {
            var task = taskResources.First();
            printer.Render(Repository, Log, task);
        }

        void ParseVariable(string variable)
        {
            var index = new[] { ':', '=' }.Select(s => variable.IndexOf(s)).Where(i => i > 0).OrderBy(i => i).FirstOrDefault();
            if (index <= 0)
                return;

            var key = variable.Substring(0, index);
            var value = (index >= variable.Length - 1) ? string.Empty : variable.Substring(index + 1);

            variables.Set(key, value);
        }
    }
}
