using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using log4net;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client.Model;
using Octopus.Client.Model.Forms;
using Octostache;

namespace Octopus.Cli.Commands
{
    public abstract class DeploymentCommandBase : ApiCommand
    {
        readonly VariableDictionary variables = new VariableDictionary();

        protected DeploymentCommandBase(IOctopusRepositoryFactory repositoryFactory, ILog log, IOctopusFileSystem fileSystem)
            : base(repositoryFactory, log, fileSystem)
        {
            SpecificMachineNames = new List<string>();
            SkipStepNames = new List<string>();
            DeployToEnvironmentNames = new List<string>();
            TenantTags = new List<string>();
            Tenants = new List<string>();

            var options = Options.For("Deployment");
            options.Add("progress", "[Optional] Show progress of the deployment", v => { showProgress = true; WaitForDeployment = true; noRawLog = true; });
            options.Add("forcepackagedownload", "[Optional] Whether to force downloading of already installed packages (flag, default false).", v => ForcePackageDownload = true);
            options.Add("waitfordeployment", "[Optional] Whether to wait synchronously for deployment to finish.", v => WaitForDeployment = true);
            options.Add("deploymenttimeout=", "[Optional] Specifies maximum time (timespan format) that the console session will wait for the deployment to finish(default 00:10:00). This will not stop the deployment. Requires --waitfordeployment parameter set.", v => DeploymentTimeout = TimeSpan.Parse(v));
            options.Add("cancelontimeout", "[Optional] Whether to cancel the deployment if the deployment timeout is reached (flag, default false).", v => CancelOnTimeout = true);
            options.Add("deploymentchecksleepcycle=", "[Optional] Specifies how much time (timespan format) should elapse between deployment status checks (default 00:00:10)", v => DeploymentStatusCheckSleepCycle = TimeSpan.Parse(v));
            options.Add("guidedfailure=", "[Optional] Whether to use Guided Failure mode. (True or False. If not specified, will use default setting from environment)", v => UseGuidedFailure = bool.Parse(v));
            options.Add("specificmachines=", "[Optional] A comma-separated list of machines names to target in the deployed environment. If not specified all machines in the environment will be considered.", v => SpecificMachineNames.AddRange(v.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(m => m.Trim())));
            options.Add("force", "[Optional] If a project is configured to skip packages with already-installed versions, override this setting to force re-deployment (flag, default false).", v => ForcePackageRedeployment = true);
            options.Add("skip=", "[Optional] Skip a step by name", v => SkipStepNames.Add(v));
            options.Add("norawlog", "[Optional] Don't print the raw log of failed tasks", v => noRawLog = true);
            options.Add("rawlogfile=", "[Optional] Redirect the raw log of failed tasks to a file", v => rawLogFile = v);
            options.Add("v|variable=", "[Optional] Values for any prompted variables in the format Label:Value", ParseVariable);
            options.Add("deployat=", "[Optional] Time at which deployment should start (scheduled deployment), specified as any valid DateTimeOffset format, and assuming the time zone is the current local time zone.", v => ParseDeployAt(v));
            options.Add("tenant=", "A tenant the deployment will be performed for; specify this argument multiple times to add multiple tenants or use `*` wildcard to deploy to tenants able to deploy.", t => Tenants.Add(t));
            options.Add("tenanttag=", "A tenant tag used to match tenants that the deployment will be performed for; specify this argument multiple times to add multiple tenant tags", tt => TenantTags.Add(tt));
        }

        protected bool ForcePackageRedeployment { get; set; }
        protected bool ForcePackageDownload { get; set; }
        protected bool? UseGuidedFailure { get; set; }
        protected bool WaitForDeployment { get; set; }
        protected TimeSpan DeploymentTimeout { get; set; }
        protected bool CancelOnTimeout { get; set; }
        protected TimeSpan DeploymentStatusCheckSleepCycle { get; set; }
        protected List<string> SpecificMachineNames { get; set; }
        protected List<string> SkipStepNames { get; set; }
        protected DateTimeOffset? DeployAt { get; set; }
        public string ProjectName { get; set; }
        public List<string> DeployToEnvironmentNames { get; set; }
        public List<string> Tenants { get; set; }
        public List<string> TenantTags { get; set; }

        private bool IsTenantedDeployment => (Tenants.Any() || TenantTags.Any());

        bool noRawLog;
        bool showProgress;
        string rawLogFile;
        TaskOutputProgressPrinter printer = new TaskOutputProgressPrinter();

        protected override void ValidateParameters()
        {
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --project=XYZ");
            if (IsTenantedDeployment && DeployToEnvironmentNames.Count > 1) throw new CommandException("Please specify only one environment at a time when deploying to tenants.");
            if (Tenants.Contains("*") && (Tenants.Count > 1 || TenantTags.Count > 0)) throw new CommandException("When deploying to all tenants using --tenant=* wildcard no other tenant filters can be provided");

            if (IsTenantedDeployment && !Repository.SupportsTenants())
                throw new CommandException("Your Octopus server does not support tenants, which was introduced in Octopus 3.4. Please upgrade your Octopus server, enable the multi-tenancy feature or remove the --tenant and --tenanttag arguments.");

            base.ValidateParameters();
        }

        DateTimeOffset? ParseDeployAt(string v)
        {
            try
            {
                return DeployAt = DateTimeOffset.Parse(v, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal);
            }
            catch (FormatException fex)
            {
                throw new CommandException($"Could not convert '{v}' to a DateTimeOffset: {fex.Message}");
            }
        }

        private List<DeploymentResource> DeployTenantedRelease(ProjectResource project, ReleaseResource release)
        {
            if (DeployToEnvironmentNames.Count != 1)
                return new List<DeploymentResource>();

            var environment = DeployToEnvironmentNames[0];
            var releaseTemplate = Repository.Releases.GetTemplate(release);
            var deploymentTenants = GetTenants(project, environment, release, releaseTemplate);
            var specificMachineIds = GetSpecificMachines();

            LogScheduledDeployment();

            return deploymentTenants.Select(tenant =>
            {
                var promotion =
                    releaseTemplate.TenantPromotions
                    .First(t => t.Id == tenant.Id).PromoteTo
                    .First(tt => tt.Name.Equals(environment, StringComparison.InvariantCultureIgnoreCase));
                return CreateDeploymentTask(project, release, promotion, specificMachineIds, tenant);
            }).ToList();
        }

        private void LogScheduledDeployment()
        {
            if (DeployAt != null)
            {
                var now = DateTimeOffset.UtcNow;
                Log.InfoFormat("Deployment will be scheduled to start in: {0}", (DeployAt.Value - now).FriendlyDuration());
            }
        }

        private ReferenceCollection GetSpecificMachines()
        {
            var specificMachineIds = new ReferenceCollection();
            if (SpecificMachineNames.Any())
            {
                var machines = Repository.Machines.FindByNames(SpecificMachineNames);
                var missing =
                    SpecificMachineNames.Except(machines.Select(m => m.Name), StringComparer.OrdinalIgnoreCase).ToList();
                if (missing.Any())
                {
                    throw new CommandException("The following specific machines could not be found: " + missing.ReadableJoin());
                }

                specificMachineIds.AddRange(machines.Select(m => m.Id));
            }
            return specificMachineIds;
        }

        protected void DeployRelease(ProjectResource project, ReleaseResource release)
        {
            var deployments = IsTenantedDeployment ?
                DeployTenantedRelease(project, release) : 
                DeployToEnvironments(project, release);

            if (deployments.Any() && WaitForDeployment)
            {
                WaitForDeploymentToComplete(deployments, project, release);
            }
        }

        private List<DeploymentResource> DeployToEnvironments(ProjectResource project, ReleaseResource release)
        {
            if (DeployToEnvironmentNames.Count == 0)
                return new List<DeploymentResource>();

            var releaseTemplate = Repository.Releases.GetTemplate(release);
            var specificMachineIds = GetSpecificMachines();

            var promotingEnvironments =
                (from environment in DeployToEnvironmentNames.Distinct(StringComparer.CurrentCultureIgnoreCase)
                    let promote = releaseTemplate.PromoteTo.FirstOrDefault(p => string.Equals(p.Name, environment))
                    select new {Name = environment, Promotion = promote}).ToList();

            var unknownEnvironments = promotingEnvironments.Where(p => p.Promotion == null).ToList();
            if (unknownEnvironments.Count > 0)
            {
                throw new CommandException(
                    string.Format(
                        "Release '{0}' of project '{1}' cannot be deployed to {2} not in the list of environments that this release can be deployed to. This may be because a) the environment does not exist, b) the name is misspelled, c) you don't have permission to deploy to this environment, or d) the environment is not in the list of environments defined by the lifecycle.",
                        release.Version,
                        project.Name,
                        unknownEnvironments.Count == 1
                            ? "environment '" + unknownEnvironments[0].Name + "' because the environment is"
                            : "environments " + string.Join(", ", unknownEnvironments.Select(e => "'" + e.Name + "'")) +
                              " because the environments are"
                        ));
            }

            LogScheduledDeployment();

            return promotingEnvironments.Select(
                    promotion => CreateDeploymentTask(project, release, promotion.Promotion, specificMachineIds))
                    .ToList();
        }

        private List<TenantResource> GetTenants(ProjectResource project, string environmentName, ReleaseResource release,
            DeploymentTemplateResource releaseTemplate)
        {
            if (!Tenants.Any() && !TenantTags.Any())
            {
                return new List<TenantResource>();
            }

            var deployableTenants = new List<TenantResource>();

            if (Tenants.Contains("*"))
            {
                var tenantPromotions = releaseTemplate.TenantPromotions.Where(
                    tp => tp.PromoteTo.Any(
                        promo => promo.Name.Equals(environmentName, StringComparison.InvariantCultureIgnoreCase))).Select(tp => tp.Id).ToArray();

                deployableTenants.AddRange(Repository.Tenants.Get(tenantPromotions));

                Log.InfoFormat("Found {0} Tenants who can deploy {1} {2} to {3}", deployableTenants.Count, project.Name,release.Version, environmentName);
            }
            else
            {
                if (Tenants.Any())
                {
                    var tenantsByName = Repository.Tenants.FindByNames(Tenants);
                    var missing = tenantsByName == null || !tenantsByName.Any()
                        ? Tenants.ToArray()
                        : Tenants.Except(tenantsByName.Select(e => e.Name), StringComparer.OrdinalIgnoreCase).ToArray();

                    var tenantsById = Repository.Tenants.Get(missing);

                    missing = tenantsById == null || !tenantsById.Any()
                        ? missing
                        : missing.Except(tenantsById.Select(e => e.Id), StringComparer.OrdinalIgnoreCase).ToArray();

                    if (missing.Any())
                        throw new ArgumentException(
                            $"Could not find the {"tenant" + (missing.Length == 1 ? "" : "s")} {string.Join(", ", missing)} on the Octopus server.");

                    deployableTenants.AddRange(tenantsByName);
                    deployableTenants.AddRange(tenantsById);

                    var unDeployableTenants =
                        deployableTenants.Where(dt => !dt.ProjectEnvironments.ContainsKey(project.Id))
                            .Select(dt => $"'{dt.Name}'")
                            .ToList();
                    if (unDeployableTenants.Any())
                        throw new CommandException(
                            string.Format(
                                "Release '{0}' of project '{1}' cannot be deployed for tenant{2} {3}. This may be because either a) {4} not connected to this project, or b) you do not have permission to deploy {5} to this project.",
                                release.Version,
                                project.Name,
                                unDeployableTenants.Count == 1 ? "" : "s",
                                string.Join(" or ", unDeployableTenants),
                                unDeployableTenants.Count == 1 ? "it is" : "they are",
                                unDeployableTenants.Count == 1 ? "it" : "them"));

                    unDeployableTenants = deployableTenants.Where(dt =>
                    {
                        var tenantPromo = releaseTemplate.TenantPromotions.FirstOrDefault(tp => tp.Id == dt.Id);
                        return tenantPromo == null ||
                               !tenantPromo.PromoteTo.Any(
                                   tdt => tdt.Name.Equals(environmentName, StringComparison.InvariantCultureIgnoreCase));
                    }).Select(dt => $"'{dt.Name}'").ToList();
                    if (unDeployableTenants.Any())
                    {
                        throw new CommandException(
                            string.Format(
                                "Release '{0}' of project '{1}' cannot be deployed for tenant{2} {3} to environment '{4}'. This may be because a) the tenant{2} {5} not connected to this environment, a) The lifecycle has not reached this phase, b) the environment does not exist, b) the environment name is misspelled, c) you don't have permission to deploy to this environment, d) the environment is not in the list of environments defined by the lifecycle, or e) {6} unable to deploy to this channel.",
                                release.Version,
                                project.Name,
                                unDeployableTenants.Count == 1 ? "" : "s",
                                string.Join(" or ", unDeployableTenants),
                                environmentName,
                                unDeployableTenants.Count == 1 ? "is" : "are",
                                unDeployableTenants.Count == 1 ? "it is" : "they are"));
                    }
                }

                if (TenantTags.Any())
                {

                    var tenantsByTag = Repository.Tenants.FindAll(null, TenantTags.ToArray());
                    var deployableByTag = tenantsByTag.Where(dt =>
                    {
                        var tenantPromo = releaseTemplate.TenantPromotions.FirstOrDefault(tp => tp.Id == dt.Id);
                        return tenantPromo != null && tenantPromo.PromoteTo.Any(tdt => tdt.Name.Equals(environmentName, StringComparison.InvariantCultureIgnoreCase));
                    }).Where(tenant => !deployableTenants.Any(deployable => deployable.Id == tenant.Id));
                    deployableTenants.AddRange(deployableByTag);
                }
            }

            if (!deployableTenants.Any())
                throw new CommandException(
                    string.Format(
                        "No tenants are available to be deployed for release '{0}' of project '{1}' to environment '{2}'.  This may be because a) No tenants matched the tags provided b) The tenants that do match are not connected to this project or environment, c) The tenants that do match are not yet able to release to this lifecycle phase, or d) you do not have the appropriate deployment permissions.",
                        release.Version, project.Name, environmentName));


            return deployableTenants;
        }

        private DeploymentResource CreateDeploymentTask(ProjectResource project, ReleaseResource release, DeploymentPromotionTarget promotionTarget, ReferenceCollection specificMachineIds, TenantResource tenant = null)
        {
            
            var preview = Repository.Releases.GetPreview(promotionTarget);

            // Validate skipped steps
            var skip = new ReferenceCollection();
            foreach (var step in SkipStepNames)
            {
                var stepToExecute =
                    preview.StepsToExecute.SingleOrDefault(s => string.Equals(s.ActionName, step, StringComparison.CurrentCultureIgnoreCase));
                if (stepToExecute == null)
                {
                    Log.WarnFormat("No step/action named '{0}' could be found when deploying to environment '{1}', so the step cannot be skipped.", step, promotionTarget.Name);
                }
                else
                {
                    Log.DebugFormat("Skipping step: {0}", stepToExecute.ActionName);
                    skip.Add(stepToExecute.ActionId);
                }
            }

            // Validate form values supplied
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

            // Log step with no machines
            foreach (var previewStep in preview.StepsToExecute)
            {
                if (previewStep.HasNoApplicableMachines)
                {
                    Log.Warn("Warning: there are no applicable machines roles used by step " + previewStep.ActionName + " step");
                }
            }

            var deployment = Repository.Deployments.Create(new DeploymentResource
            {
                TenantId = tenant?.Id,
                EnvironmentId = promotionTarget.Id,
                SkipActions = skip,
                ReleaseId = release.Id,
                ForcePackageDownload = ForcePackageDownload,
                UseGuidedFailure = UseGuidedFailure.GetValueOrDefault(preview.UseGuidedFailureModeByDefault),
                SpecificMachineIds = specificMachineIds,
                ForcePackageRedeployment = ForcePackageRedeployment,
                FormValues = (preview.Form ?? new Form()).Values,
                QueueTime = DeployAt == null ? null : (DateTimeOffset?) DeployAt.Value
            });

            Log.InfoFormat("Deploying {0} {1} to: {2} {3}(Guided Failure: {4})", project.Name, release.Version, promotionTarget.Name,
                tenant == null ? string.Empty : $"for {tenant.Name} ",
                deployment.UseGuidedFailure ? "Enabled" : "Not Enabled");

            return deployment;
        }

        public void WaitForDeploymentToComplete(List<DeploymentResource> deployments, ProjectResource project, ReleaseResource release)
        {
            var deploymentTasks = deployments.Select(dep => Repository.Tasks.Get(dep.TaskId)).ToList();
            if (showProgress && deployments.Count > 1)
            {
                Log.InfoFormat("Only progress of the first task ({0}) will be shown", deploymentTasks.First().Name);
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

                        if (noRawLog)
                        {
                            continue;
                        }

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
                if (failed)
                {
                    throw new CommandException("One or more deployment tasks failed.");
                }

                Log.Info("Done!");
            }
            catch (TimeoutException e)
            {
                Log.Error(e.Message);

                CancelDeploymentOnTimeoutIfRequested(deploymentTasks);

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

        private void CancelDeploymentOnTimeoutIfRequested(List<TaskResource> deploymentTasks)
        {
            if (!CancelOnTimeout)
                return;

            deploymentTasks.ForEach(task => {
                Log.WarnFormat("Cancelling deployment task '{0}'", task.Description);
                try
                {
                    Repository.Tasks.Cancel(task);
                }
                catch(Exception ex)
                {
                    Log.ErrorFormat("Failed to cancel deployment task '{0}': {1}", task.Description, ex.Message);
                }
            });
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
