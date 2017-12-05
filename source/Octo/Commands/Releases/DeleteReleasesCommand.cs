using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;

namespace Octopus.Cli.Commands.Releases
{
    [Command("delete-releases", Description = "Deletes a range of releases")]
    public class DeleteReleasesCommand : ApiCommand, ISupportFormattedOutput
    {
        ProjectResource project;
        HashSet<string> channels;
        ResourceCollection<ReleaseResource> releases;
        List<ReleaseResource> toDelete;
        List<ReleaseResource> wouldDelete;

        public DeleteReleasesCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
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

        public async Task Request()
        {
            if (ChannelNames.Any() && !Repository.SupportsChannels()) throw new CommandException("Your Octopus server does not support channels, which was introduced in Octopus 3.2. Please upgrade your Octopus server, or remove the --channel arguments.");
            if (string.IsNullOrWhiteSpace(ProjectName)) throw new CommandException("Please specify a project name using the parameter: --project=XYZ");
            if (string.IsNullOrWhiteSpace(MinVersion)) throw new CommandException("Please specify a minimum version number using the parameter: --minversion=X.Y.Z");
            if (string.IsNullOrWhiteSpace(MaxVersion)) throw new CommandException("Please specify a maximum version number using the parameter: --maxversion=X.Y.Z");

            var min = SemanticVersion.Parse(MinVersion);
            var max = SemanticVersion.Parse(MaxVersion);

            
            project = await GetProject().ConfigureAwait(false);
            var channelsTask = GetChannelIds(project);
            releases = await Repository.Projects.GetReleases(project).ConfigureAwait(false);
            
            channels = await channelsTask.ConfigureAwait(false);

            commandOutputProvider.Debug("Finding releases for project...");
            
            toDelete = new List<ReleaseResource>();
            wouldDelete = new List<ReleaseResource>();
            await releases.Paginate(Repository, page =>
            {
                foreach (var release in page.Items)
                {
                    if (channels.Any() && !channels.Contains(release.ChannelId))
                        continue;

                    var version = SemanticVersion.Parse(release.Version);
                    if (min > version || version > max)
                        continue;

                    if (WhatIf)
                    {
                        commandOutputProvider.Information("[Whatif] Version {Version:l} would have been deleted", version);
                        wouldDelete.Add(release);
                    }
                    else
                    {
                        toDelete.Add(release);
                        commandOutputProvider.Information("Deleting version {Version:l}", version);
                    }
                }

                // We need to consider all releases
                return true;
            })
            .ConfigureAwait(false);

            // Don't do anything else for WhatIf
            if (WhatIf) return;

            foreach (var release in toDelete)
            {
                await Repository.Client.Delete(release.Link("Self")).ConfigureAwait(false);
            }
        }

        private async Task<HashSet<string>> GetChannelIds(ProjectResource project)
        {
            if (ChannelNames.None())
                return new HashSet<string>();

            commandOutputProvider.Debug("Finding channels: {Channels:l}", ChannelNames.CommaSeperate());

            var firstChannelPage = await Repository.Projects.GetChannels(project).ConfigureAwait(false);
            var allChannels = await firstChannelPage.GetAllPages(Repository).ConfigureAwait(false);
            var channels = allChannels.Where(c => ChannelNames.Contains(c.Name, StringComparer.CurrentCultureIgnoreCase))
                .ToArray();

            var notFoundChannels = ChannelNames.Except(channels.Select(c => c.Name), StringComparer.CurrentCultureIgnoreCase).ToArray();
            if (notFoundChannels.Any())
                throw new CouldNotFindException("the channels named", notFoundChannels.CommaSeperate());

            return new HashSet<string>(channels.Select(c => c.Id));
        }

        private async Task<ProjectResource> GetProject()
        {
            commandOutputProvider.Debug("Finding project: {Project:l}", ProjectName);
            var project = await Repository.Projects.FindByName(ProjectName).ConfigureAwait(false);
            if (project == null)
                throw new CouldNotFindException("a project named", ProjectName);
            return project;
        }

        public void PrintDefaultOutput()
        {

        }

        public void PrintJsonOutput()
        {
            List<ReleaseResource> affectedReleases = WhatIf ? wouldDelete : toDelete;
            commandOutputProvider.Json(new
            {
                Project = new { project.Id, project.Name },
                Releases = affectedReleases.Select(r => new
                    {
                        r.Version,
                        Deleted = toDelete.Any(x=>x.Version == r.Version)
                    }
                )
            });
        }
    }
}