using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OctopusTools.Client;
using OctopusTools.Diagnostics;
using OctopusTools.Infrastructure;
using log4net;

namespace OctopusTools.Commands
{
    [Command("create-release", Description = "Creates and (optionally) deploys a release")]
    public class CreateReleaseCommand : ApiCommand
    {
        readonly IDeploymentWatcher deploymentWatcher;
        readonly IPackageVersionResolver versionResolver;

        public CreateReleaseCommand(IOctopusSessionFactory session, ILog log, IDeploymentWatcher deploymentWatcher, IPackageVersionResolver versionResolver)
            : base(session, log)
        {
            this.deploymentWatcher = deploymentWatcher;
            this.versionResolver = versionResolver;

            DeployToEnvironmentNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            DeploymentStatusCheckSleepCycle = TimeSpan.FromSeconds(10);
            DeploymentTimeout = TimeSpan.FromMinutes(10);
        }

        public string ProjectName { get; set; }
        public HashSet<string> DeployToEnvironmentNames { get; set; }
        public string VersionNumber { get; set; }
        public string ReleaseNotes { get; set; }
        public bool Force { get; set; }
        public bool ForcePackageDownload { get; set; }
        public bool WaitForDeployment { get; set; }
        public TimeSpan DeploymentTimeout { get; set; }
        public TimeSpan DeploymentStatusCheckSleepCycle { get; set; }

        protected override void SetOptions(OptionSet options)
        {
            options.Add("project=", "Name of the project", v => ProjectName = v);
            options.Add("deployto=", "[Optional] Environment to automatically deploy to, e.g., Production", v => DeployToEnvironmentNames.Add(v));
            options.Add("releaseNumber=|version=", "Release number to use for the new release.", v => VersionNumber = v);
            options.Add("defaultpackageversion=|packageversion=", "Default version number of all packages to use for this release.", v => versionResolver.Default(v));
            options.Add("package=|packageversionoverride=", "[Optional] Version number to use for a package in the release. Format: --package={PackageId}:{Version}", v => versionResolver.Add(v));
            options.Add("packagesFolder=", "[Optional] A folder containing NuGet packages from which we should get versions.", v => versionResolver.AddFolder(v));
            options.Add("forceversion", "Ignored (obsolete).", v => { });
            options.Add("force", "Whether to force redeployment of already installed packages (flag, default false).", v => Force = true);
            options.Add("forcepackagedownload", "Whether to force downloading of already installed packages (flag, default false).", v => ForcePackageDownload = true);
            options.Add("releasenotes=", "Release Notes for the new release.", v => ReleaseNotes = v);
            options.Add("releasenotesfile=", "Path to a file that contains Release Notes for the new release.", ReadReleaseNotesFromFile);
            options.Add("waitfordeployment", "Whether to wait synchronously for deployment to finish.", v => WaitForDeployment = true);
            options.Add("deploymenttimeout=", "[Optional] Specifies maximum time (timespan format) that deployment can take (default 00:10:00)", v => DeploymentTimeout = TimeSpan.Parse(v));
            options.Add("deploymentchecksleepcycle=", "[Optional] Specifies how much time (timespan format) should elapse between deployment status checks (default 00:00:10)", v => DeploymentStatusCheckSleepCycle = TimeSpan.Parse(v));
        }

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --project=XYZ");

            Log.Debug("Finding project: " + ProjectName);
            var project = Session.GetProject(ProjectName);

            Log.Debug("Finding environments...");
            var environments = Session.FindEnvironments(DeployToEnvironmentNames);

            Log.Debug("Finding steps for project...");
            var steps = Session.FindStepsForProject(project);

            var plan = new ReleasePlan(steps, versionResolver);

            if (plan.UnresolvedSteps.Count > 0)
            {
                Log.Debug("Resolving NuGet package versions...");
                foreach (var unresolved in plan.UnresolvedSteps)
                {
                    Log.Debug("  - Finding latest NuGet package for step: " + unresolved.StepName);
                    unresolved.SetVersionFromLatest(Session.GetLatestPackageForStep(unresolved.Step).NuGetPackageVersion);
                }
            }

            var versionNumber = VersionNumber;
            if (string.IsNullOrWhiteSpace(versionNumber))
            {
                Log.Warn("A --version parameter was not specified, so a version number was automatically selected based on the highest package version.");
                versionNumber = plan.GetHighestVersionNumber();
            }

            Log.Info("Release plan for release:    " + versionNumber);
            Log.Info("Steps: ");
            Log.Info(plan.FormatAsTable());

            Log.Debug("Creating release...");
            var release = Session.CreateRelease(project, plan.GetSelections(), versionNumber, ReleaseNotes);
            Log.Info("Release created successfully!");

            Log.ServiceMessage("setParameter", new { name = "octo.releaseNumber", value = release.Version });

            if (environments == null || environments.Count <= 0) return;
            var linksToDeploymentTasks = Session.GetDeployments(release, environments, Force, ForcePackageDownload, Log).ToList();

            if (WaitForDeployment)
            {
                deploymentWatcher.WaitForDeploymentsToFinish(Session, linksToDeploymentTasks, DeploymentTimeout, DeploymentStatusCheckSleepCycle);
            }
        }

        private void ReadReleaseNotesFromFile(string value)
        {
            try
            {
                ReleaseNotes = File.ReadAllText(value);
            }
            catch (IOException ex)
            {
                throw new CommandException(ex.Message);
            }
        }
    }
}
