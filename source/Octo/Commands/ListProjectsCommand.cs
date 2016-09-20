using System;
using System.Threading.Tasks;
using Serilog;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;

namespace Octopus.Cli.Commands
{
    [Command("list-projects", Description = "Lists all projects")]
    public class ListProjectsCommand : ApiCommand
    {
        public ListProjectsCommand(IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory)
            : base(clientFactory, repositoryFactory, log, fileSystem)
        {
        }

        protected override async Task Execute()
        {
            var projects = await Repository.Projects.FindAll().ConfigureAwait(false);

            Log.Information("Projects: " + projects.Count);

            foreach (var project in projects)
            {
                Log.Information(" - {0} (ID: {1})", project.Name, project.Id);
            }
        }
    }
}