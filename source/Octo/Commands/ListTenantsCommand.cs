using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Octo.Commands;
using Serilog;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    [Command("list-tenants", Description = "List tenants")]
    public class ListTenantsCommand : ApiCommand, ISupportFormattedOutput 
    {
        List<TenantResource> tenants;

        public ListTenantsCommand(IOctopusAsyncRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory,ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, log, fileSystem, commandOutputProvider)
        {
        }

        public async Task Query()
        {
            tenants = await Repository.Tenants.FindAll().ConfigureAwait(false);
        }

        public void PrintDefaultOutput()
        {
            Log.Information("Tenants: {Count}", tenants.Count);

            foreach (var tenant in tenants.OrderBy(m => m.Name))
            {
                Log.Information(" - {Tenant:l} (ID: {Count})", tenant.Name, tenant.Id);
            }
        }

        public void PrintJsonOutput()
        {
            Log.Information(JsonConvert.SerializeObject(tenants.OrderBy(x => x.Name).Select(t => new
            {
                t.Name,
                t.Id
            }), Formatting.Indented));

        }

        public void PrintXmlOutput()
        {
            throw new NotImplementedException();
        }
    }
}