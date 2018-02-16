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
    [Command("list-releases", Description = "List releases by project")]
    public class ListReleasesCommand : ApiCommand, ISupportFormattedOutput
    {
        readonly HashSet<string> projects = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private List<ProjectResource> projectResources;
        private string[] projectsFilter;
        List<ReleaseResource> releases;

        public ListReleasesCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
        {
            var options = Options.For("Listing");
            options.Add("project=", "Name of a project to filter by. Can be specified many times.", v => projects.Add(v));
        }


        public async Task Request()
        {
            projectResources = new List<ProjectResource>();
            projectsFilter = new string[0];

            if (projects.Count > 0)
            {
                commandOutputProvider.Debug("Loading projects...");
                //var test = Repository.Projects.FindByNames(projects.ToArray());
                projectResources = await Repository.Projects.FindByNames(projects.ToArray()).ConfigureAwait(false);
                projectsFilter = projectResources.Select(p => p.Id).ToArray();
            }

            commandOutputProvider.Debug("Loading releases...");
            
            releases = await Repository.Releases
                .FindMany(x => projectsFilter.Contains(x.ProjectId))
                .ConfigureAwait(false);
        }

        public void PrintDefaultOutput()
        {
            commandOutputProvider.Information("Releases: {Count}", releases.Count);
            foreach (var project in projectResources)
            {
                commandOutputProvider.Information(" - Project: {Project:l}", project.Name);

                foreach (var release in releases.Where(x => x.ProjectId == project.Id))
                {
                    var propertiesToLog = new List<string>();
                    propertiesToLog.AddRange(FormatReleasePropertiesAsStrings(release));
                    foreach (var property in propertiesToLog)
                    {
                        commandOutputProvider.Information("    {Property:l}", property);
                    }
                    commandOutputProvider.Information("");
                }
            }
        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(projectResources.Select(pr => new
            {
                Project = new { pr.Id, pr.Name },
                Releases = releases.Where(r => r.ProjectId == pr.Id).Select(r => new
                {
                    r.Version,
                    r.Assembled,
                    PackageVersions = GetPackageVersionsAsString(r.SelectedPackages),
                    ReleaseNotes = GetReleaseNotes(r)
                })
            }));
        }
    }
}
