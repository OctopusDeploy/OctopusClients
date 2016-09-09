using System;
using Serilog;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;

namespace Octopus.Cli.Commands
{
    [Command("list-environments", Description = "List environments")]
    public class ListEnvironmentsCommand : ApiCommand
    {

        public ListEnvironmentsCommand(IOctopusRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem)
            : base(repositoryFactory, log, fileSystem)
        {
        }

        protected override void Execute()
        {
            var environments = Repository.Environments.FindAll();
            Log.Information("Environments: " + environments.Count);

            foreach (var environment in environments)
            {
                Log.Information(" - {0} (ID: {1})", environment.Name, environment.Id);
            }
        }
    }
}