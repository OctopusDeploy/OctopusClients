using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Octopus.Client.Model;

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

        public void DeployRelease(ProjectResource project, ReleaseResource release, List<EnvironmentResource> environments)
        {
            var deployments = new List<DeploymentResource>();
            var deploymentTasks = new List<TaskResource>();
            Log.InfoFormat("Deploying {0} {1} to:", project.Name, release.Version);
            foreach (var environment in environments)
            {
                var deployment = Repository.Deployments.Create(new DeploymentResource
                {
                    EnvironmentId = environment.Id,
                    ReleaseId = release.Id,
                    ForcePackageDownload = ForcePackageDownload,
                    UseGuidedFailure = UseGuidedFailure.GetValueOrDefault(environment.UseGuidedFailure)
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
