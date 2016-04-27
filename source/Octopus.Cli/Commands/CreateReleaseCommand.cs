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
            options.Add("autochannel", "[Optional] Automatically calculate Channel based on version matching rules", v => AutoChannel = true);
            options.Add("version=|releaseNumber=", "[Optional] Release number to use for the new release.", v => VersionNumber = v);
            options.Add("packageversion=|defaultpackageversion=", "Default version number of all packages to use for this release.", v => versionResolver.Default(v));
            options.Add("package=", "[Optional] Version number to use for a package in the release. Format: --package={StepName}:{Version}", v => versionResolver.Add(v));
            options.Add("packagesFolder=", "[Optional] A folder containing NuGet packages from which we should get versions.", v => versionResolver.AddFolder(v));
            options.Add("releasenotes=", "[Optional] Release Notes for the new release.", v => ReleaseNotes = v);
            options.Add("releasenotesfile=", "[Optional] Path to a file that contains Release Notes for the new release.", ReadReleaseNotesFromFile);
            options.Add("ignoreexisting", "If a release with the version number already exists, ignore it", v => IgnoreIfAlreadyExists = true);
            options.Add("ignorechannelrules", "[Optional] Ignore package version matching rules", v => Force = true);
            options.Add("packageprerelease=", "[Optional] Pre-release for latest version of all packages to use for this release.", v => VersionPreReleaseTag = v);
            options.Add("whatif", "[Optional] Perform a dry run but don't actually create/deploy release.", v => WhatIf = true);

            options = Options.For("Deployment");
            options.Add("deployto=", "[Optional] Environment to automatically deploy to, e.g., Production", v => DeployToEnvironmentNames.Add(v));
        }

        public string ProjectName { get; set; }
        public string ChannelName { get; set; }
        public bool AutoChannel { get; set; }
        public List<string> DeployToEnvironmentNames { get; set; }
        public string VersionNumber { get; set; }
        public string ReleaseNotes { get; set; }
        public bool IgnoreIfAlreadyExists { get; set; }
        public bool Force { get; set; }
        public string VersionPreReleaseTag { get; set; }
        public bool WhatIf { get; set; }

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --project=XYZ");

            if (!Repository.Client.RootDocument.HasLink("Channels") && (AutoChannel || !string.IsNullOrWhiteSpace(ChannelName))) throw new CommandException("Your Octopus server does not support channels, which was introduced in Octopus 3.2. Either upgrade your Octopus server, or create the release without any channel arguments.");
            if (AutoChannel && !string.IsNullOrWhiteSpace(ChannelName)) throw new CommandException("Cannot specify --channel and --autochannel arguments");
            if (AutoChannel && Force) throw new CommandException("Cannot specify --autochannel and --ignorechannelrules arguments - the channel rules are how we select the most suitable channel");

            if (WhatIf) Log.Info("What if: no release will be created.");

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
                Log.Error($"Release plan for {ProjectName} {versionNumber}{Environment.NewLine}{plan.FormatAsTable()}");
            }

            if (plan.HasUnresolvedSteps())
            {
                throw new CommandException("Package versions could not be resolved for one or more of the package steps in this release. See the errors above for details. Either ensure the latest version of the package can be automatically resolved, or set the version to use specifically by using the --package argument.");
            }

            if (plan.HasStepsViolatingChannelVersionRules())
            {
                if (Force)
                {
                    Log.Warn($"At least one step violates the package version rules for the Channel '{plan.Channel.Name}'. Forcing the release to be created ignoring these rules...");
                }

                throw new CommandException($"At least one step violates the package version rules for the Channel '{plan.Channel.Name}'. Either correct the package versions for this release, select a different channel using --channel=MyChannel argument, let Octopus select the best channel using the --autoChannel argument, or ignore these version rules altogether by using the --ignoreChannelRules argument.");
            }

            if (IgnoreIfAlreadyExists)
            {
                Log.Debug("Checking for existing release...");
                try
                {
                    var found = Repository.Projects.GetReleaseByVersion(project, versionNumber);
                    if (found != null)
                    {
                        Log.Info("A release with the number " + versionNumber + " already exists.");
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
                Log.InfoFormat("What if: not creating this release");
            }
            else
            {
                // Actually create the release!
                Log.Debug("Creating release...");
                var release = Repository.Releases.Create(new ReleaseResource(versionNumber, project.Id, plan.Channel?.Id)
                {
                    ReleaseNotes = ReleaseNotes,
                    SelectedPackages = plan.GetSelections()
                }, Force);
                Log.Info("Release " + release.Version + " created successfully!");
                Log.ServiceMessage("setParameter", new { name = "octo.releaseNumber", value = release.Version });
                Log.TfsServiceMessage(ServerBaseUrl, project, release);

                DeployRelease(project, release, DeployToEnvironmentNames);
            }
        }

        private ReleasePlan BuildReleasePlan(ProjectResource project)
        {
            ReleasePlan plan;

            if (AutoChannel)
            {
                Log.Debug("Automatically selecting the best channel for this release...");
                plan = AutoSelectBestReleasePlanOrThrow(project);
            }
            else if (!string.IsNullOrWhiteSpace(ChannelName))
            {
                Log.Info("Building release plan for channel: " + ChannelName);
                var channels = Repository.Projects.GetChannels(project);
                var matchingChannel =
                    channels.Items.SingleOrDefault(c => c.Name.Equals(ChannelName, StringComparison.OrdinalIgnoreCase));
                if (matchingChannel == null)
                    throw new CouldNotFindException($"a channel in {project.Name} named", ChannelName);

                plan = releasePlanBuilder.Build(Repository, project, matchingChannel, VersionPreReleaseTag);
            }
            else
            {
                plan = releasePlanBuilder.Build(Repository, project, null, VersionPreReleaseTag);
            }

            return plan;
        }

        ReleasePlan AutoSelectBestReleasePlanOrThrow(ProjectResource project)
        {
            // Build a release plan for each channel to determine which channel is the best match for the provided options
            var candidateChannels = Repository.Projects.GetChannels(project).Items;
            var releasePlans = new List<ReleasePlan>();
            foreach (var channel in candidateChannels)
            {
                if (channel.Rules.Count <= 0)
                {
                    Log.Debug($"Channel '{channel.Name}' has no version rules, skipping...");
                    continue;
                }

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
                    $"{string.Join(Environment.NewLine, releasePlans.Select(p => p.FormatAsTable()))}");
            }

            if (viablePlans.Length == 1)
            {
                var selectedPlan = viablePlans.Single();
                Log.Info($"Selected the release plan for Channel '{selectedPlan.Channel.Name}' - it is a perfect match");
                return selectedPlan;
            }

            throw new CommandException(
                $"There are {viablePlans.Length} viable release plans using the provided arguments so we cannot auto-select one. The viable release plans are:" +
                Environment.NewLine +
                $"{string.Join(Environment.NewLine, viablePlans.Select(p => p.FormatAsTable()))}" +
                Environment.NewLine +
                "The unviable release plans are:" +
                Environment.NewLine +
                $"{string.Join(Environment.NewLine, releasePlans.Except(viablePlans).Select(p => p.FormatAsTable()))}");
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