using System;
using System.Threading.Tasks;
using Serilog;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;

namespace Octopus.Cli.Commands
{
    [Command("list-environments", Description = "List environments")]
    public class ListEnvironmentsCommand : ApiCommand
    {

        public ListEnvironmentsCommand(IOctopusRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory)
            : base(clientFactory, repositoryFactory, log, fileSystem)
        {
        }

        protected override async Task Execute()
        {
            var environments = await Repository.Environments.FindAll().ConfigureAwait(false);
            Log.Information("Environments: " + environments.Count);

            foreach (var environment in environments)
            {
                Log.Information(" - {0} (ID: {1})", environment.Name, environment.Id);
            }
        }
    }
}