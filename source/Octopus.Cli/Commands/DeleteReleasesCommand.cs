using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    [Command("delete-releases", Description = "Deletes a range of releases")]
    public class DeleteReleasesCommand : ApiCommand
    {
        public DeleteReleasesCommand(IOctopusRepositoryFactory repositoryFactory, ILog log, IOctopusFileSystem fileSystem)
            : base(repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Deletion");
            options.Add("project=", "Name of the project", v => ProjectName = v);
            options.Add("minversion=", "Minimum (inclusive) version number for the range of versions to delete", v => MinVersion = v);
            options.Add("maxversion=", "Maximum (inclusive) version number for the range of versions to delete", v => MaxVersion = v);
            options.Add("channel=", "[Optional] if specified, only releases associated with the channel will be deleted; specify this argument multiple times to target multiple channels.", v => ChannelNames.Add(v));
            options.Add("whatif", "[Optional, Flag] if specified, releases won't actually be deleted, but will be listed as if simulating the command", v => WhatIf = true);
        }

        public string ProjectName { get; set; }
        public string MaxVersion { get; set; }
        public string MinVersion { get; set; }
        public List<string> ChannelNames { get; } = new List<string>();
        public bool WhatIf { get; set; }

        protected override void Execute()
        {
            if (ChannelNames.Any() && !Repository.SupportsChannels()) throw new CommandException("Your Octopus server does not support channels, which was introduced in Octopus 3.2. Please upgrade your Octopus server, or remove the --channel arguments.");
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --project=XYZ");
            if (string.IsNullOrWhiteSpace(MinVersion)) throw new CommandException("Please specify a minimum version number using the parameter: --minversion=X.Y.Z");
            if (string.IsNullOrWhiteSpace(MaxVersion)) throw new CommandException("Please specify a maximum version number using the parameter: --maxversion=X.Y.Z");

            var min = SemanticVersion.Parse(MinVersion);
            var max = SemanticVersion.Parse(MaxVersion);

            var project = GetProject();
            var channels = GetChannelIds(project);

            Log.Debug("Finding releases for project...");

            var releases = Repository.Projects.GetReleases(project);
            var toDelete = new List<string>();
            while (releases.Items.Count > 0)
            {
                foreach (var release in releases.Items)
                {
                    if (channels.Any() && !channels.Contains(release.ChannelId))
                        continue;

                    var version = SemanticVersion.Parse(release.Version);
                    if (min > version || version > max)
                        continue;

                    if (WhatIf)
                    {
                        Log.InfoFormat("[Whatif] Version {0} would have been deleted", version);
                    }
                    else
                    {
                        toDelete.Add(release.Link("Self"));
                        Log.InfoFormat("Deleting version {0}", version);
                    }
                }

                if (!releases.HasLink("Page.Next"))
                {
                    break;
                }

                releases = Repository.Client.List<ReleaseResource>(releases.Link("Page.Next"));
            }

            if (!WhatIf)
            {
                foreach (var release in toDelete)
                {
                    Repository.Client.Delete(release);
                }
            }
        }

        private HashSet<string> GetChannelIds(ProjectResource project)
        {
            if (ChannelNames.None())
                return new HashSet<string>();

            Log.Debug("Finding channels: " + ChannelNames.CommaSeperate());

            var channels = GetAllChannelsFor(project)
                .Where(c => ChannelNames.Contains(c.Name, StringComparer.InvariantCultureIgnoreCase))
                .ToArray();

            var notFoundChannels = ChannelNames.Except(channels.Select(c => c.Name), StringComparer.InvariantCultureIgnoreCase).ToArray();
            if(notFoundChannels.Any())
                throw new CouldNotFindException("the channels named", notFoundChannels.CommaSeperate());

            return channels.Select(c => c.Id).ToHashSet();
        }

        private IEnumerable<ChannelResource> GetAllChannelsFor(ProjectResource project)
        {
            var channelCollection = Repository.Projects.GetChannels(project);
            foreach (var channel in channelCollection.Items)
                yield return channel;

            while (channelCollection.HasLink("Page.Next"))
            {
                channelCollection = Repository.Client.List<ChannelResource>(channelCollection.Link("Page.Next"));
                foreach (var channel in channelCollection.Items)
                    yield return channel;
            }
        }

        private ProjectResource GetProject()
        {
            Log.Debug("Finding project: " + ProjectName);
            var project = Repository.Projects.FindByName(ProjectName);
            if (project == null)
                throw new CouldNotFindException("a project named", ProjectName);
            return project;
        }
    }
}