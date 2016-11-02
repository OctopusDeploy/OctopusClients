using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    [Command("list-releases", Description = "List releases by project")]
    public class ListReleasesCommand : ApiCommand
    {
        readonly HashSet<string> projects = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public ListReleasesCommand(IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory)
            : base(clientFactory, repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Listing");
            options.Add("project=", "Name of a project to filter by. Can be specified many times.", v => projects.Add(v));
        }

        protected override async Task Execute()
        {
            var projectResources = new List<ProjectResource>();
            var projectsFilter = new string[0];
            if (projects.Count > 0)
            {
                Log.Debug("Loading projects...");
                //var test = Repository.Projects.FindByNames(projects.ToArray());
                projectResources = await Repository.Projects.FindByNames(projects.ToArray()).ConfigureAwait(false);
                projectsFilter = projectResources.Select(p => p.Id).ToArray();
            }

            Log.Debug("Loading releases...");
            var releases = await Repository.Releases
                .FindMany(x => projectsFilter.Contains(x.ProjectId))
                .ConfigureAwait(false);

            Log.Information("Releases: {Count}", releases.Count);

            foreach (var project in projectResources)
            {
                Log.Information(" - Project: {Project:l}", project.Name);
                
                foreach (var release in releases.Where(x => x.ProjectId == project.Id))
                {
                    var propertiesToLog = new List<string>();
                    propertiesToLog.AddRange(FormatReleasePropertiesAsStrings(release));
                    foreach (var property in propertiesToLog)
                    {
                        Log.Information("    {Property:l}", property);
                    }
                    Log.Information("");
                }
            }
        }
    }
}
