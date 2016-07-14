using System;
using System.Linq;
using log4net;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;

namespace Octopus.Cli.Commands
{
    [Command("list-tenants", Description = "List tenants")]
    public class ListTenantsCommand : ApiCommand
    {

        public ListTenantsCommand(IOctopusRepositoryFactory repositoryFactory, ILog log, IOctopusFileSystem fileSystem)
            : base(repositoryFactory, log, fileSystem)
        {
        }

        protected override void Execute()
        {
            var tenants = Repository.Tenants.FindAll();
            Log.Info("Tenants: " + tenants.Count);

            foreach (var tenant in tenants.OrderBy(m => m.Name))
            {
                Log.InfoFormat(" - {0} (ID: {1})", tenant.Name, tenant.Id);
            }
        }
    }
}