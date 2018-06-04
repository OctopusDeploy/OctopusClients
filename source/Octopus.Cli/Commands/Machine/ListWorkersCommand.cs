using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Cli.Commands.Machine
{
    [Command("list-workers", Description = "Lists all workers")]
    public class ListWorkersCommand : ApiCommand, ISupportFormattedOutput
    {
        readonly HashSet<string> pools = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> statuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> healthStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private HealthStatusProvider provider;
        List<WorkerPoolResource> workerpoolResources;
        IEnumerable<WorkerResource> workerpoolWorkers;
        private bool? isDisabled;
        private bool? isCalamariOutdated;
        private bool? isTentacleOutdated;

        public ListWorkersCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
        {
            var options = Options.For("Listing Workers");
            options.Add("workerpool=", "Name of a worker pool to filter by. Can be specified many times.", v => pools.Add(v));
            options.Add("status=", $"[Optional] Status of Machines filter by ({string.Join(", ", HealthStatusProvider.StatusNames)}). Can be specified many times.", v => statuses.Add(v));
            options.Add("health-status=|healthstatus=", $"[Optional] Health status of Machines filter by ({string.Join(", ", HealthStatusProvider.HealthStatusNames)}). Can be specified many times.", v => healthStatuses.Add(v));
            options.Add("disabled=", "[Optional] Disabled status filter of Machine.", v => SetFlagState(v, ref isDisabled));
            options.Add("calamari-outdated=", "[Optional] State of Calamari to filter. By default ignores Calamari state.", v => SetFlagState(v, ref isCalamariOutdated));
            options.Add("tentacle-outdated=", "[Optional] State of Tentacle version to filter. By default ignores Tentacle state", v => SetFlagState(v, ref isTentacleOutdated));
        }

        public async Task Request()
        {
            provider = new HealthStatusProvider(Repository, statuses, healthStatuses, commandOutputProvider);

            workerpoolResources = await GetPools().ConfigureAwait(false);

            workerpoolWorkers = await FilterByWorkerPools(workerpoolResources).ConfigureAwait(false);
            workerpoolWorkers = FilterByState(workerpoolWorkers, provider);
        }

        public void PrintDefaultOutput()
        {
            LogFilteredMachines(workerpoolWorkers, provider, workerpoolResources);
        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(workerpoolWorkers.Select(machine => new
            {
                machine.Id,
                machine.Name,
                Status = provider.GetStatus(machine),
                WorkerPools = machine.WorkerPoolIds.Select(id => workerpoolResources.First(e => e.Id == id).Name)
                    .ToArray()
            }));
        }

        private void LogFilteredMachines(IEnumerable<WorkerResource> poolMachines, HealthStatusProvider provider, List<WorkerPoolResource> poolResources)
        {
            var orderedMachines = poolMachines.OrderBy(m => m.Name).ToList();
            commandOutputProvider.Information("Workers: {Count}", orderedMachines.Count);
            foreach (var machine in orderedMachines)
            {
                commandOutputProvider.Information(" - {Machine:l} {Status:l} (ID: {MachineId:l}) in {WorkerPool:l}", machine.Name, provider.GetStatus(machine), machine.Id,
                    string.Join(" and ", machine.WorkerPoolIds.Select(id => poolResources.First(e => e.Id == id).Name)));
            }
        }

        private Task<List<WorkerPoolResource>> GetPools()
        {
            commandOutputProvider.Debug("Loading pools...");
            return Repository.WorkerPools.FindAll();
        }

        private IEnumerable<WorkerResource> FilterByState(IEnumerable<WorkerResource> poolMachines, HealthStatusProvider provider)
        {
            poolMachines = provider.Filter(poolMachines);

            if (isDisabled.HasValue)
            {
                poolMachines = poolMachines.Where(m => m.IsDisabled == isDisabled.Value);
            }
            if (isCalamariOutdated.HasValue)
            {
                poolMachines = poolMachines.Where(m => m.HasLatestCalamari == !isCalamariOutdated.Value);
            }
            if (isTentacleOutdated.HasValue)
            {
                poolMachines =
                    poolMachines.Where(
                        m =>
                            (m.Endpoint as ListeningTentacleEndpointResource)?.TentacleVersionDetails.UpgradeSuggested ==
                            isTentacleOutdated.Value);
            }
            return poolMachines;
        }

        private  Task<List<WorkerResource>> FilterByWorkerPools(List<WorkerPoolResource> poolResources)
        {
            var poolsToInclude = poolResources.Where(e => pools.Contains(e.Name, StringComparer.OrdinalIgnoreCase)).ToList();
            var missingPools = pools.Except(poolsToInclude.Select(e => e.Name), StringComparer.OrdinalIgnoreCase).ToList();
            if (missingPools.Any())
                throw new CouldNotFindException("pools(s) named", string.Join(", ", missingPools));


            var poolsFilter = poolsToInclude.Select(p => p.Id).ToList();

            commandOutputProvider.Debug("Loading workers...");
            if (poolsFilter.Count > 0)
            {
                commandOutputProvider.Debug("Loading machines from {WorkerPools:l}...", string.Join(", ", poolsToInclude.Select(e => e.Name)));
                return
                    Repository.Workers.FindMany(
                        x => { return x.WorkerPoolIds.Any(poolId => poolsFilter.Contains(poolId)); });
            }
            else
            {
                commandOutputProvider.Debug("Loading workers from all pools...");
                return  Repository.Workers.FindAll();
            }
        }
    }
}