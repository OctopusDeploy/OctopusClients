using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using Octopus.Cli.Diagnostics;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    [Command("create-release", Description = "Creates (and, optionally, deploys) a release.")]
    public class CreateReleaseCommand : DeploymentCommandBase
    {
        private readonly IReleasePlanBuilder releasePlanBuilder;

        public CreateReleaseCommand(IOctopusRepositoryFactory repositoryFactory, ILog log, IOctopusFileSystem fileSystem, IPackageVersionResolver versionResolver, IReleasePlanBuilder releasePlanBuilder)
            : base(repositoryFactory, log, fileSystem)
        {
            this.releasePlanBuilder = releasePlanBuilder;

            DeployToEnvironmentNames = new List<string>();
            DeploymentStatusCheckSleepCycle = TimeSpan.FromSeconds(10);
            DeploymentTimeout = TimeSpan.FromMinutes(10);

            var options = Options.For("Release creation");
            options.Add("project=", "Name of the project", v => ProjectName = v);
            options.Add("channel=", "[Optional] Channel to use for the new release.", v => ChannelName = v);
            options.Add("version=|releaseNumber=", "[Optional] Release number to use for the new release.", v => VersionNumber = v);
            options.Add("packageversion=|defaultpackageversion=", "Default version number of all packages to use for this release.", v => versionResolver.Default(v));
            options.Add("package=", "[Optional] Version number to use for a package in the release. Format: --package={StepName}:{Version}", v => versionResolver.Add(v));
            options.Add("packagesFolder=", "[Optional] A folder containing NuGet packages from which we should get versions.", v => versionResolver.AddFolder(v));
            options.Add("releasenotes=", "[Optional] Release Notes for the new release.", v => ReleaseNotes = v);
            options.Add("releasenotesfile=", "[Optional] Path to a file that contains Release Notes for the new release.", ReadReleaseNotesFromFile);
            options.Add("ignoreexisting", "If a release with the version number already exists, ignore it", v => IgnoreIfAlreadyExists = true);
            options.Add("ignorechannelrules", "[Optional] Ignore package version matching rules", v => IgnoreChannelRules = true);
            options.Add("packageprerelease=", "[Optional] Pre-release for latest version of all packages to use for this release.", v => VersionPreReleaseTag = v);
            options.Add("whatif", "[Optional] Perform a dry run but don't actually create/deploy release.", v => WhatIf = true);

            options = Options.For("Deployment");
            options.Add("deployto=", "[Optional] Environment to automatically deploy to, e.g., Production", v => DeployToEnvironmentNames.Add(v));
        }

        public string ProjectName { get; set; }
        public string ChannelName { get; set; }
        public List<string> DeployToEnvironmentNames { get; set; }
        public string VersionNumber { get; set; }
        public string ReleaseNotes { get; set; }
        public bool IgnoreIfAlreadyExists { get; set; }
        public bool IgnoreChannelRules { get; set; }
        public string VersionPreReleaseTag { get; set; }
        public bool WhatIf { get; set; }

        protected override void Execute()
        {
            if (!string.IsNullOrWhiteSpace(ChannelName) && !Repository.SupportsChannels()) throw new CommandException("Your Octopus server does not support channels, which was introduced in Octopus 3.2. Please upgrade your Octopus server, or remove the --channel argument.");
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --project=XYZ");

            Log.DebugFormat("This Octopus Server {0} channels", ServerSupportsChannels() ? "supports" : "does not support");

            Log.Debug("Finding project: " + ProjectName);
            var project = Repository.Projects.FindByName(ProjectName);
            if (project == null)
                throw new CouldNotFindException("a project named", ProjectName);

            var plan = BuildReleasePlan(project);

            string versionNumber;
            if (!string.IsNullOrWhiteSpace(VersionNumber))
            {
                versionNumber = VersionNumber;
                Log.Debug("Using version number provided on command-line: " + versionNumber);
            }
            else if (!string.IsNullOrWhiteSpace(plan.ReleaseTemplate.NextVersionIncrement))
            {
                versionNumber = plan.ReleaseTemplate.NextVersionIncrement;
                Log.Debug("Using version number from release template: " + versionNumber);
            }
            else if (!string.IsNullOrWhiteSpace(plan.ReleaseTemplate.VersioningPackageStepName))
            {
                versionNumber = plan.GetActionVersionNumber(plan.ReleaseTemplate.VersioningPackageStepName);
                Log.Debug("Using version number from package step: " + versionNumber);
            }
            else
            {
                throw new CommandException("A version number was not specified and could not be automatically selected.");
            }
            
            if (plan.IsViableReleasePlan())
            {
                Log.Info($"Release plan for {ProjectName} {versionNumber}{Environment.NewLine}{plan.FormatAsTable()}");
            }
            else
            {
                Log.Warn($"Release plan for {ProjectName} {versionNumber}{Environment.NewLine}{plan.FormatAsTable()}");
            }

            if (plan.HasUnresolvedSteps())
            {
                throw new CommandException("Package versions could not be resolved for one or more of the package steps in this release. See the errors above for details. Either ensure the latest version of the package can be automatically resolved, or set the version to use specifically by using the --package argument.");
            }

            if (plan.HasStepsViolatingChannelVersionRules())
            {
                if (IgnoreChannelRules)
                {
                    Log.Warn($"At least one step violates the package version rules for the Channel '{plan.Channel.Name}'. Forcing the release to be created ignoring these rules...");
                }
                else
                {
                    throw new CommandException($"At least one step violates the package version rules for the Channel '{plan.Channel.Name}'. Either correct the package versions for this release, let Octopus select the best channel by omitting the --channel argument, select a different channel using --channel=MyChannel argument, or ignore these version rules altogether by using the --ignoreChannelRules argument.");
                }
            }

            if (IgnoreIfAlreadyExists)
            {
                Log.Debug($"Checking for existing release for {ProjectName} {versionNumber} because you specified --ignoreexisting...");
                try
                {
                    var found = Repository.Projects.GetReleaseByVersion(project, versionNumber);
                    if (found != null)
                    {
                        Log.Info($"A release of {ProjectName} with the number {versionNumber} already exists, and you specified --ignoreexisting, so we won't even attempt to create the release.");
                        return;
                    }
                }
                catch (OctopusResourceNotFoundException)
                {
                    // Expected
                    Log.Debug("No release exists - the coast is clear!");
                }
            }

            if (WhatIf)
            {
                // We were just doing a dry run - bail out here
                Log.InfoFormat("[WhatIf] This release would have been created using the release plan{0}",
                    DeployToEnvironmentNames.Any() ? $" and deployed to {DeployToEnvironmentNames.CommaSeperate()}" : string.Empty);
            }
            else
            {
                // Actually create the release!
                Log.Debug("Creating release...");
                var release = Repository.Releases.Create(new ReleaseResource(versionNumber, project.Id, plan.Channel?.Id)
                {
                    ReleaseNotes = ReleaseNotes,
                    SelectedPackages = plan.GetSelections()
                }, ignoreChannelRules: IgnoreChannelRules);
                Log.Info($"Release {release.Version} created successfully!");
                Log.ServiceMessage("setParameter", new { name = "octo.releaseNumber", value = release.Version });
                Log.TfsServiceMessage(ServerBaseUrl, project, release);

                DeployRelease(project, release, DeployToEnvironmentNames);
            }
        }

        private ReleasePlan BuildReleasePlan(ProjectResource project)
        {
            if (!string.IsNullOrWhiteSpace(ChannelName))
            {
                Log.Info($"Building release plan for channel '{ChannelName}'...");
                var channels = Repository.Projects.GetChannels(project);
                var matchingChannel =
                    channels.Items.SingleOrDefault(c => c.Name.Equals(ChannelName, StringComparison.OrdinalIgnoreCase));
                if (matchingChannel == null)
                    throw new CouldNotFindException($"a channel in {project.Name} named", ChannelName);

                return releasePlanBuilder.Build(Repository, project, matchingChannel, VersionPreReleaseTag);
            }

            // All Octopus 3.2+ servers should have the Channels hypermedia link, we should use the channel information
            // to select the most appropriate channel, or provide enough information to proceed from here
            if (ServerSupportsChannels())
            {
                Log.Debug("Automatically selecting the best channel for this release...");
                return AutoSelectBestReleasePlanOrThrow(project);
            }
            
            // Compatibility: this has to cater for Octopus before Channels existed
            Log.Info("Building release plan without a channel for Octopus Server without channels support...");
            return releasePlanBuilder.Build(Repository, project, null, VersionPreReleaseTag);
        }

        private bool ServerSupportsChannels()
        {
            return Repository.Client.RootDocument.HasLink("Channels");
        }

        ReleasePlan AutoSelectBestReleasePlanOrThrow(ProjectResource project)
        {
            // Build a release plan for each channel to determine which channel is the best match for the provided options
            var candidateChannels = Repository.Projects.GetChannels(project).GetAllPages(Repository);
            var releasePlans = new List<ReleasePlan>();
            foreach (var channel in candidateChannels)
            {
                Log.Info($"Building a release plan for Channel '{channel.Name}'...");

                var plan = releasePlanBuilder.Build(Repository, project, channel, VersionPreReleaseTag);
                releasePlans.Add(plan);
            }

            var viablePlans = releasePlans.Where(p => p.IsViableReleasePlan()).ToArray();
            if (viablePlans.Length <= 0)
            {
                throw new CommandException(
                    "There are no viable release plans in any channels using the provided arguments. The following release plans were considered:" +
                    Environment.NewLine +
                    $"{releasePlans.Select(p => p.FormatAsTable()).NewlineSeperate()}");
            }

            if (viablePlans.Length == 1)
            {
                var selectedPlan = viablePlans.Single();
                Log.Info($"Selected the release plan for Channel '{selectedPlan.Channel.Name}' - it is a perfect match");
                return selectedPlan;
            }

            if (viablePlans.Length > 1 && viablePlans.Any(p => p.Channel.IsDefault))
            {
                var selectedPlan = viablePlans.First(p => p.Channel.IsDefault);
                Log.Info($"Selected the release plan for Channel '{selectedPlan.Channel.Name}' - there were multiple matching Channels ({viablePlans.Select(p => p.Channel.Name).CommaSeperate()}) so we selected the default channel.");
                return selectedPlan;
            }

            throw new CommandException(
                $"There are {viablePlans.Length} viable release plans using the provided arguments so we cannot auto-select one. The viable release plans are:" +
                Environment.NewLine +
                $"{viablePlans.Select(p => p.FormatAsTable()).NewlineSeperate()}" +
                Environment.NewLine +
                "The unviable release plans are:" +
                Environment.NewLine +
                $"{releasePlans.Except(viablePlans).Select(p => p.FormatAsTable()).NewlineSeperate()}");
        }

        void ReadReleaseNotesFromFile(string value)
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