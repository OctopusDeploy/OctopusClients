using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using OctopusTools.Client;
using OctopusTools.Infrastructure;
using OctopusTools.Model;
using log4net;

namespace OctopusTools.Commands
{
    public class CreateReleaseCommand : ApiCommand
    {
        readonly IDeploymentWatcher deploymentWatcher;

        public CreateReleaseCommand(IOctopusSession session, ILog log, IDeploymentWatcher deploymentWatcher): base(session, log)
        {
            this.deploymentWatcher = deploymentWatcher;
            DeployToEnvironmentNames = new List<string>();
            DeploymentStatusCheckSleepCycle = TimeSpan.FromSeconds(10);
        }

        public string ProjectName { get; set; }
        public IList<string> DeployToEnvironmentNames { get; set; }
        public string VersionNumber { get; set; }
        public string ReleaseNotes { get; set; }
        public bool Force { get; set; }
        public TimeSpan? WaitUntilDeploymentIsFinishedTimeout { get; set; }
        public TimeSpan DeploymentStatusCheckSleepCycle { get; set; }

        public override OptionSet Options
        {
            get
            {
                var options = base.Options;
                options.Add("project=", "Name of the project", v => ProjectName = v);
                options.Add("deployto=", "[Optional] Environment to automatically deploy to, e.g., Production", v => DeployToEnvironmentNames.Add(v));
                options.Add("version=", "Version number to use for the new release.", v => VersionNumber = v);
                options.Add("force", "Whether to force redeployment of already installed packages (flag, default false).", v => Force = true);
                options.Add("releasenotes=", "Release Notes for the new release.", v => ReleaseNotes = v);
                options.Add("deploymenttimeout=", "If this value is specified the tool will block and wait synchronously for deployment to finish.", v => WaitUntilDeploymentIsFinishedTimeout = TimeSpan.Parse(v));
                options.Add("deploymentchecksleepcycle=", "[Optional] Specifies how much time should elapse between deployment status checks", v => DeploymentStatusCheckSleepCycle = TimeSpan.Parse(v));

                return options;
            }
        }

        public override void Execute()
        {
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --project=XYZ");

            Log.Debug("Finding project: " + ProjectName);
            var project = Session.GetProject(ProjectName);

            Log.Debug("Finding environments...");
            var environments = Session.FindEnvironments(DeployToEnvironmentNames);

            Log.Debug("Finding steps for project...");
            var steps = Session.FindStepsForProject(project);

            Log.Debug("Getting latest package versions for each step...");
            var selected = new List<SelectedPackage>();
            foreach (var step in steps)
            {
                var version = Session.GetLatestPackageForStep(step);
                Log.DebugFormat("{0} - latest: {1}", step.Description, version.NuGetPackageVersion);
                selected.Add(version);
            }

            var versionNumber = VersionNumber;
            if (string.IsNullOrWhiteSpace(versionNumber))
            {
                versionNumber = selected.Select(p => SemanticVersion.Parse(p.NuGetPackageVersion)).OrderByDescending(v => v).First().ToString();
                Log.Warn("A --version parameter was not specified, so a version number was automatically selected based on the highest package version: " + versionNumber);
            }

            Log.Debug("Creating release: " + versionNumber);
            var release = Session.CreateRelease(project, selected, versionNumber, ReleaseNotes);
            Log.Info("Release created successfully!");

            if (environments != null)
            {
                var linksToDeploymentTasks = RequestDeployments(release, environments);

                if (WaitUntilDeploymentIsFinishedTimeout != null)
                {

                    deploymentWatcher.WaitForDeploymentsToFinish(linksToDeploymentTasks, WaitUntilDeploymentIsFinishedTimeout.Value, DeploymentStatusCheckSleepCycle);
                }
            }
        }

        List<string> RequestDeployments(Release release, IEnumerable<DeploymentEnvironment> environments)
        {
            var linksToDeploymentTasks = new List<string>();
            foreach (var environment in environments)
            {
                var deployment = Session.DeployRelease(release, environment, Force);
                var linkToTask = deployment.Links.Single(l => l.Key == "Task").Value;
                linksToDeploymentTasks.Add(linkToTask);

                Log.InfoFormat("Successfully scheduled release '{0}' for deployment to environment '{1}'" + deployment.Name, release.Version, environment.Name);
            }
            return linksToDeploymentTasks;
        }
    }
}
