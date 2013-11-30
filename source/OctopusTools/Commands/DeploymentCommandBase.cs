using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Octopus.Client.Model;
using OctopusTools.Infrastructure;

namespace OctopusTools.Commands
{
    public abstract class DeploymentCommandBase : ApiCommand
    {
        protected DeploymentCommandBase(IOctopusRepositoryFactory repositoryFactory, ILog log) : base(repositoryFactory, log)
        {
        }

        protected virtual void SetCommonOptions(OptionSet options)
        {
            options.Add("forcepackagedownload", "Whether to force downloading of already installed packages (flag, default false).", v => ForcePackageDownload = true);
            options.Add("waitfordeployment", "Whether to wait synchronously for deployment to finish.", v => WaitForDeployment = true);
            options.Add("deploymenttimeout=", "[Optional] Specifies maximum time (timespan format) that deployment can take (default 00:10:00)", v => DeploymentTimeout = TimeSpan.Parse(v));
            options.Add("deploymentchecksleepcycle=", "[Optional] Specifies how much time (timespan format) should elapse between deployment status checks (default 00:00:10)", v => DeploymentStatusCheckSleepCycle = TimeSpan.Parse(v));
            options.Add("guidedfailure=", "[Optional] Whether to use Guided Failure mode. (True or False. If not specified, will use default setting from environment)", v => UseGuidedFailure = bool.Parse(v));
        }


        protected virtual bool ForcePackageDownload { get; set; }
        protected virtual bool? UseGuidedFailure { get; set; }
        protected virtual bool WaitForDeployment { get; set; }
        protected virtual TimeSpan DeploymentTimeout { get; set; }
        protected virtual TimeSpan DeploymentStatusCheckSleepCycle { get; set; }

        public void DeployRelease(ProjectResource project, ReleaseResource release, List<string> environments)
        {
            if (environments.Count == 0)
                return;

            var deployments = new List<DeploymentResource>();
            var deploymentTasks = new List<TaskResource>();
            Log.InfoFormat("Deploying {0} {1} to:", project.Name, release.Version);

            var releaseTemplate = Repository.Releases.GetTemplate(release);

            var promotingEnvironments =
                (from environment in environments.Distinct(StringComparer.CurrentCultureIgnoreCase)
                 let promote = releaseTemplate.PromoteTo.FirstOrDefault(p => string.Equals(p.Name, environment))
                 select new {Name = environment, Promote = promote}).ToList();

            var unknownEnvironments = promotingEnvironments.Where(p => p.Promote == null).ToList();
            if (unknownEnvironments.Count > 0)
            {
                throw new CommandException(string.Format("Release '{0}' of project '{1}' cannot be deployed to {2} not in the list of environments that this release can be deployed to. This may be because a) the environment does not exist, b) the name is misspelled, c) you don't have permission to deploy to this environment, or d) the environment is not in the list of environments defined by the project group.", 
                    release.Version, 
                    project.Name, 
                    unknownEnvironments.Count == 1 ? "environment '" + unknownEnvironments[0].Name + "' because the environment is"
                    : "environments " + string.Join(", ", unknownEnvironments.Select(e => "'" + e.Name + "'")) + " because the environments are"
                    ));
            }

            foreach (var environment in promotingEnvironments)
            {
                var promote = environment.Promote;
                var preview = Repository.Releases.GetPreview(promote);

                var deployment = Repository.Deployments.Create(new DeploymentResource
                {
                    EnvironmentId = promote.Id,
                    ReleaseId = release.Id,
                    ForcePackageDownload = ForcePackageDownload,
                    UseGuidedFailure = UseGuidedFailure.GetValueOrDefault(preview.UseGuidedFailureModeByDefault)
                });

                deployments.Add(deployment);
                deploymentTasks.Add(Repository.Tasks.Get(deployment.TaskId));
                Log.InfoFormat("  - {0} (Guided Failure: {1})", environment.Name, deployment.UseGuidedFailure ? "Enabled" : "Not Enabled");
            }

            if (WaitForDeployment)
            {
                WaitForDeploymentToComplete(deploymentTasks, deployments, project, release);
            }
        }

        public void WaitForDeploymentToComplete(List<TaskResource> deploymentTasks, List<DeploymentResource> deployments, ProjectResource project, ReleaseResource release)
        {
            try
            {
                Log.InfoFormat("Waiting for {0} deployment(s) to complete....", deploymentTasks.Count);
                Repository.Tasks.WaitForCompletion(deploymentTasks.ToArray(), DeploymentStatusCheckSleepCycle.Seconds, DeploymentTimeout.Minutes);
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
            }
        }
    }
}
