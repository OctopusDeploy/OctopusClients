using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands.WorkerPool
{
    [Command("list-workerpools", Description = "List worker pools")]
    public class ListWorkerPoolsCommand : ApiCommand, ISupportFormattedOutput
    {
        List<WorkerPoolResource> pools;

        public ListWorkerPoolsCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
        {
        }

        public async Task Request()
        {
            pools = await Repository.WorkerPools.FindAll().ConfigureAwait(false);
        }

        public void PrintDefaultOutput()
        {
            commandOutputProvider.Information("WorkerPools: {Count}", pools.Count);

            foreach (var pool in pools)
            {
                commandOutputProvider.Information(" - {WorkerPools:l} (ID: {Id:l})", pool.Name, pool.Id);
            }
        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(
                pools.Select(pool => new
                {
                    pool.Id,
                    pool.Name
                }));
        }
    }
}