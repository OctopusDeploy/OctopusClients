using System;
using Serilog;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;

namespace Octopus.Cli.Commands
{
    [Command("list-projects", Description = "Lists all projects")]
    public class ListProjectsCommand : ApiCommand
    {
        public ListProjectsCommand(IOctopusRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem)
            : base(repositoryFactory, log, fileSystem)
        {
        }

        protected override void Execute()
        {
            var projects = Repository.Projects.FindAll();

            Log.Information("Projects: " + projects.Count);

            foreach (var project in projects)
            {
                Log.Information(" - {0} (ID: {1})", project.Name, project.Id);
            }
        }
    }
}