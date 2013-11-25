using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet;
using Octopus.Client.Model;
using OctopusTools.Diagnostics;
using OctopusTools.Infrastructure;
using log4net;

namespace OctopusTools.Commands
{
    [Command("create-release", Description = "Creates (and, optionally, deploys) a release.")]
    public class CreateReleaseCommand : DeploymentCommandBase
    {
        readonly IPackageVersionResolver versionResolver;

        public CreateReleaseCommand(IOctopusRepositoryFactory repositoryFactory, ILog log, IPackageVersionResolver versionResolver)
            : base(repositoryFactory, log)
        {
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

        protected override void SetOptions(OptionSet options)
        {
            SetCommonOptions(options);
            options.Add("project=", "Name of the project", v => ProjectName = v);
            options.Add("deployto=", "[Optional] Environment to automatically deploy to, e.g., Production", v => DeployToEnvironmentNames.Add(v));
            options.Add("releaseNumber=|version=", "Release number to use for the new release.", v => VersionNumber = v);
            options.Add("defaultpackageversion=|packageversion=", "Default version number of all packages to use for this release.", v => versionResolver.Default(v));
            options.Add("package=|packageversionoverride=", "[Optional] Version number to use for a package in the release. Format: --package={PackageId}:{Version}", v => versionResolver.Add(v));
            options.Add("packagesFolder=", "[Optional] A folder containing NuGet packages from which we should get versions.", v => versionResolver.AddFolder(v));
            options.Add("forceversion", "Ignored (obsolete).", v => { });
            options.Add("force", "Whether to force redeployment of already installed packages (flag, default false).", v => Force = true);
            options.Add("releasenotes=", "Release Notes for the new release.", v => ReleaseNotes = v);
            options.Add("releasenotesfile=", "Path to a file that contains Release Notes for the new release.", ReadReleaseNotesFromFile);
        }

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --project=XYZ");

            Log.Debug("Finding project: " + ProjectName);
            var project = Repository.Projects.FindByName(ProjectName);

            Log.Debug("Finding deployment process for project: " + ProjectName);
            var deploymentProcess = Repository.DeploymentProcesses.Get(project.DeploymentProcessId);

            Log.Debug("Finding environments...");
            var environments = Repository.Environments.FindByNames(DeployToEnvironmentNames);

            Log.Debug("Finding tasks for project...");

            var steps = deploymentProcess.Steps;

            var plan = new ReleasePlan(steps, versionResolver);

            if (plan.UnresolvedSteps.Count > 0)
            {
                Log.Debug("Resolving NuGet package versions...");
                foreach (var unresolved in plan.UnresolvedSteps)
                {
                    Log.Debug("  - Finding latest NuGet package for step: " + unresolved.StepName);
                    var version = Repository.Client.Get<List<PackageResource>>("~/api/feeds/{feedId}/packages?packageIds={packageId}", new {feedId = unresolved.NuGetFeedId, packageId = unresolved.PackageId})[0].Version;
                    unresolved.SetVersionFromLatest(version);
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
            var release = Repository.Releases.Create(new ReleaseResource(VersionNumber, project.Id)
            {
                ReleaseNotes = ReleaseNotes,
                SelectedPackages = plan.GetSelections()
            });
            Log.Info("Release " + release.Version + " created successfully!");

            Log.ServiceMessage("setParameter", new { name = "octo.releaseNumber", value = release.Version });

            if (environments == null || environments.Count <= 0) return;
            DeployRelease(project, release, environments);
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
