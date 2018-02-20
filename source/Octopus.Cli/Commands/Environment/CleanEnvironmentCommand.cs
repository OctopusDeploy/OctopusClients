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
using Serilog;

#pragma warning disable 618

namespace Octopus.Cli.Commands.Environment
{
    [Command("clean-environment", Description = "Cleans all Offline Machines from an Environment")]
    public class CleanEnvironmentCommand : ApiCommand, ISupportFormattedOutput
    {
        string environmentName;
        readonly HashSet<string> statuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> healthStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private bool? isDisabled;
        private bool? isCalamariOutdated;
        private bool? isTentacleOutdated;
        EnvironmentResource environmentResource;
        IEnumerable<MachineResource> machines;
        List<MachineResult> commandResults = new List<MachineResult>();


        public CleanEnvironmentCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
        {
            var options = Options.For("Cleanup");
            options.Add("environment=", "Name of an environment to clean up.", v => environmentName = v);
            options.Add("status=", $"Status of Machines clean up ({string.Join(", ", HealthStatusProvider.StatusNames)}). Can be specified many times.", v => statuses.Add(v));
            options.Add("health-status=|healthstatus=", $"Health status of Machines to clean up ({string.Join(", ", HealthStatusProvider.HealthStatusNames)}). Can be specified many times.", v => healthStatuses.Add(v));
            options.Add("disabled=", "[Optional] Disabled status filter of Machine to clean up.", v => SetFlagState(v, ref isDisabled));
            options.Add("calamari-outdated=", "[Optional] State of Calamari to clean up. By default ignores Calamari state.", v => SetFlagState(v, ref isCalamariOutdated));
            options.Add("tentacle-outdated=", "[Optional] State of Tentacle version to clean up. By default ignores Tentacle state", v => SetFlagState(v, ref isTentacleOutdated));
        }

        public async Task Request()
        {
            if (string.IsNullOrWhiteSpace(environmentName))
                throw new CommandException("Please specify an environment name using the parameter: --environment=XYZ");
            if (!healthStatuses.Any() && !statuses.Any())
                throw new CommandException("Please specify a status using the parameter: --status or --health-status");

            environmentResource = await GetEnvironment().ConfigureAwait(false);

            machines = await FilterByEnvironment(environmentResource).ConfigureAwait(false);
            machines = FilterByState(machines);

            await CleanUpEnvironment(machines.ToList(), environmentResource);
        }

        private async Task CleanUpEnvironment(List<MachineResource> filteredMachines, EnvironmentResource environmentResource)
        {
            commandOutputProvider.Information("Found {MachineCount} machines in {Environment:l} with the status {Status:l}", filteredMachines.Count, environmentResource.Name, GetStateFilterDescription());

            if (filteredMachines.Any(m => m.EnvironmentIds.Count > 1))
            {
                commandOutputProvider.Information("Note: Some of these machines belong to multiple environments. Instead of being deleted, these machines will be removed from the {Environment:l} environment.", environmentResource.Name);
            }

            foreach (var machine in filteredMachines)
            {
                MachineResult result = new MachineResult
                {
                    Machine = machine
                };
                // If the machine belongs to more than one environment, we should remove the machine from the environment rather than delete it altogether.
                if (machine.EnvironmentIds.Count > 1)
                {
                    commandOutputProvider.Information("Removing {Machine:l} {Status} (ID: {Id:l}) from {Environment:l}", machine.Name, machine.Status, machine.Id,
                        environmentResource.Name);
                    machine.EnvironmentIds.Remove(environmentResource.Id);
                    await Repository.Machines.Modify(machine).ConfigureAwait(false);
                    result.Action = MachineAction.RemovedFromEnvironment;
                }
                else
                {
                    commandOutputProvider.Information("Deleting {Machine:l} {Status} (ID: {Id:l})", machine.Name, machine.Status, machine.Id);
                    await Repository.Machines.Delete(machine).ConfigureAwait(false);
                    result.Action = MachineAction.Deleted;
                }

                this.commandResults.Add(result);
            }
        }

        private IEnumerable<MachineResource> FilterByState(IEnumerable<MachineResource> environmentMachines)
        {
            var provider = new HealthStatusProvider(Repository, statuses, healthStatuses, commandOutputProvider);
            environmentMachines = provider.Filter(environmentMachines);

            if (isDisabled.HasValue)
            {
                environmentMachines = environmentMachines.Where(m => m.IsDisabled == isDisabled.Value);
            }
            if (isCalamariOutdated.HasValue)
            {
                environmentMachines = environmentMachines.Where(m => m.HasLatestCalamari == !isCalamariOutdated.Value);
            }
            if (isTentacleOutdated.HasValue)
            {
                environmentMachines = environmentMachines.Where(m => (m.Endpoint as ListeningTentacleEndpointResource)?.TentacleVersionDetails.UpgradeSuggested == isTentacleOutdated.Value);
            }
            return environmentMachines;
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

        private Task<List<MachineResource>> FilterByEnvironment(EnvironmentResource environmentResource)
        {
            commandOutputProvider.Debug("Loading machines...");
            return Repository.Machines.FindMany(x =>  x.EnvironmentIds.Any(environmentId => environmentId == environmentResource.Id));
        }

        private async Task<EnvironmentResource> GetEnvironment()
        {
            commandOutputProvider.Debug("Loading environments...");
            var environmentResource = await Repository.Environments.FindByName(environmentName).ConfigureAwait(false);
            if (environmentResource == null)
            {
                throw new CouldNotFindException("the specified environment");
            }
            return environmentResource;
        }

        public void PrintDefaultOutput()
        {

        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(commandResults.Select(x =>new
            {
                Machine = new { x.Machine.Id,x.Machine.Name, x.Machine.Status },
                Environment = x.Action == MachineAction.RemovedFromEnvironment ? new { environmentResource.Id, environmentResource.Name } : null,
                Action = x.Action.ToString()
            }));
        }

        public enum MachineAction
        {
            RemovedFromEnvironment,
            Deleted
        }

        private class MachineResult
        {
            public MachineResource Machine { get; set; }
            public MachineAction Action { get; set; }
        }
    }
}
