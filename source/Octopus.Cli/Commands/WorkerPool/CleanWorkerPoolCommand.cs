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

namespace Octopus.Cli.Commands.WorkerPools
{
    [Command("clean-workerpool", Description = "Cleans all Offline Worker Machines from a WorkerPool")]
    public class CleanWorkerPoolCommand : ApiCommand, ISupportFormattedOutput
    {
        string poolName;
        readonly HashSet<string> statuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> healthStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private bool? isDisabled;
        private bool? isCalamariOutdated;
        private bool? isTentacleOutdated;
        WorkerPoolResource workerpoolResource;
        IEnumerable<WorkerMachineResource> machines;
        List<MachineResult> commandResults = new List<MachineResult>();


        public CleanWorkerPoolCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
        {
            var options = Options.For("WorkerPool Cleanup");
            options.Add("workerpool=", "Name of a worker pool to clean up.", v => poolName = v);
            options.Add("status=", $"Status of Worker Machines to clean up ({string.Join(", ", HealthStatusProvider.StatusNames)}). Can be specified many times.", v => statuses.Add(v));
            options.Add("health-status=", $"Health status of Worker Machines to clean up ({string.Join(", ", HealthStatusProvider.HealthStatusNames)}). Can be specified many times.", v => healthStatuses.Add(v));
            options.Add("disabled=", "[Optional] Disabled status filter of Worker Machine to clean up.", v => SetFlagState(v, ref isDisabled));
            options.Add("calamari-outdated=", "[Optional] State of Calamari to clean up. By default ignores Calamari state.", v => SetFlagState(v, ref isCalamariOutdated));
            options.Add("tentacle-outdated=", "[Optional] State of Tentacle version to clean up. By default ignores Tentacle state", v => SetFlagState(v, ref isTentacleOutdated));
        }

        public async Task Request()
        {
            if (string.IsNullOrWhiteSpace(poolName))
                throw new CommandException("Please specify a worker pool name using the parameter: --workerpool=XYZ");
            if (!healthStatuses.Any() && !statuses.Any())
                throw new CommandException("Please specify a status using the parameter: --health-status");

            workerpoolResource = await GetWorkerPool().ConfigureAwait(false);

            machines = await FilterByWorkerPool(workerpoolResource).ConfigureAwait(false);
            machines = FilterByState(machines);

            await CleanUpPool(machines.ToList(), workerpoolResource);
        }

        private async Task CleanUpPool(List<WorkerMachineResource> filteredMachines, WorkerPoolResource poolResource)
        {
            commandOutputProvider.Information("Found {MachineCount} machines in {WorkerPool:l} with the status {Status:l}", filteredMachines.Count, poolResource.Name, GetStateFilterDescription());

            if (filteredMachines.Any(m => m.WorkerPoolIds.Count > 1))
            {
                commandOutputProvider.Information("Note: Some of these machines belong to multiple pools. Instead of being deleted, these machines will be removed from the {WorkerPool:l} pool.", poolResource.Name);
            }

            foreach (var machine in filteredMachines)
            {
                MachineResult result = new MachineResult
                {
                    Machine = machine
                };
                // If the machine belongs to more than one pool, we should remove the machine from the pool rather than delete it altogether.
                if (machine.WorkerPoolIds.Count > 1)
                {
                    commandOutputProvider.Information("Removing {Machine:l} {Status} (ID: {Id:l}) from {WorkerPool:l}", machine.Name, machine.Status, machine.Id,
                        poolResource.Name);
                    machine.WorkerPoolIds.Remove(poolResource.Id);
                    await Repository.WorkerMachines.Modify(machine).ConfigureAwait(false);
                    result.Action = MachineAction.RemovedFromPool;
                }
                else
                {
                    commandOutputProvider.Information("Deleting {Machine:l} {Status} (ID: {Id:l})", machine.Name, machine.Status, machine.Id);
                    await Repository.WorkerMachines.Delete(machine).ConfigureAwait(false);
                    result.Action = MachineAction.Deleted;
                }

                this.commandResults.Add(result);
            }
        }

        private IEnumerable<WorkerMachineResource> FilterByState(IEnumerable<WorkerMachineResource> workerMachines)
        {
            var provider = new HealthStatusProvider(Repository, statuses, healthStatuses, commandOutputProvider);
            workerMachines = provider.Filter(workerMachines);

            if (isDisabled.HasValue)
            {
                workerMachines = workerMachines.Where(m => m.IsDisabled == isDisabled.Value);
            }
            if (isCalamariOutdated.HasValue)
            {
                workerMachines = workerMachines.Where(m => m.HasLatestCalamari == !isCalamariOutdated.Value);
            }
            if (isTentacleOutdated.HasValue)
            {
                workerMachines = workerMachines.Where(m => (m.Endpoint as ListeningTentacleEndpointResource)?.TentacleVersionDetails.UpgradeSuggested == isTentacleOutdated.Value);
            }
            return workerMachines;
        }

        private string GetStateFilterDescription()
        {
            var description =  string.Join(",", statuses.Concat(healthStatuses));

            if (isDisabled.HasValue)
            {
                description += isDisabled.Value ? "and disabled" : "and not disabled";
            }

            if (isCalamariOutdated.HasValue)
            {
                description += $" and its Calamari version {(isCalamariOutdated.Value ? "" : "not")}out of date";
            }

            if (isTentacleOutdated.HasValue)
            {
                description += $" and its Tentacle version {(isTentacleOutdated.Value ? "" : "not")}out of date";
            }

            return description;
        }

        private Task<List<WorkerMachineResource>> FilterByWorkerPool(WorkerPoolResource poolResource)
        {
            commandOutputProvider.Debug("Loading worker machines...");
            return Repository.WorkerMachines.FindMany(x =>  x.WorkerPoolIds.Any(poolId => poolId == poolResource.Id));
        }

        private async Task<WorkerPoolResource> GetWorkerPool()
        {
            commandOutputProvider.Debug("Loading worker pools...");
            var poolResource = await Repository.WorkerPools.FindByName(poolName).ConfigureAwait(false);
            if (poolResource == null)
            {
                throw new CouldNotFindException("the specified worker pool");
            }
            return poolResource;
        }

        public void PrintDefaultOutput()
        {

        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(commandResults.Select(x =>new
            {
                Machine = new { x.Machine.Id,x.Machine.Name, x.Machine.Status },
                Environment = x.Action == MachineAction.RemovedFromPool ? new { workerpoolResource.Id, workerpoolResource.Name } : null,
                Action = x.Action.ToString()
            }));
        }

        public enum MachineAction
        {
            RemovedFromPool,
            Deleted
        }

        private class MachineResult
        {
            public MachineBasedResource Machine { get; set; }
            public MachineAction Action { get; set; }
        }
    }
}