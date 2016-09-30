using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;

namespace Octopus.Cli.Commands
{
    [Command("list-tenants", Description = "List tenants")]
    public class ListTenantsCommand : ApiCommand
    {

        public ListTenantsCommand(IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory)
            : base(clientFactory, repositoryFactory, log, fileSystem)
        {
        }

        protected override async Task Execute()
        {
            var tenants = await Repository.Tenants.FindAll().ConfigureAwait(false);
            Log.Information("Tenants: " + tenants.Count);

            foreach (var tenant in tenants.OrderBy(m => m.Name))
            {
                Log.Information(" - {0} (ID: {1})", tenant.Name, tenant.Id);
            }
        }
    }
}