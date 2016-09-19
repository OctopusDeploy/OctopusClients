using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

#pragma warning disable 618

namespace Octopus.Cli.Commands
{
    [Command("clean-environment", Description = "Cleans all Offline Machines from an Environment")]
    public class CleanEnvironmentCommand : ApiCommand
    {
        string environmentName;
        readonly HashSet<string> statuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> healthStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private bool? isDisabled;
        private bool? isCalamariOutdated;
        private bool? isTentacleOutdated;

        public CleanEnvironmentCommand(IOctopusRepositoryFactory repositoryFactory, ILogger log, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory)
            : base(clientFactory, repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Cleanup");
            options.Add("environment=", "Name of an environment to clean up.", v => environmentName = v);
            options.Add("status=", $"Status of Machines clean up ({string.Join(", ", HealthStatusProvider.StatusNames)}). Can be specified many times.", v => statuses.Add(v));
            options.Add("health-status=|healthstatus=", $"Health status of Machines to clean up ({string.Join(", ", HealthStatusProvider.HealthStatusNames)}). Can be specified many times.", v => healthStatuses.Add(v));
            options.Add("disabled=", "[Optional] Disabled status filter of Machine to clean up.", v => SetFlagState(v, ref isDisabled));
            options.Add("calamari-outdated=", "[Optional] State of Calamari to clean up. By default ignores Calamari state.", v => SetFlagState(v, ref isCalamariOutdated));
            options.Add("tentacle-outdated=", "[Optional] State of Tentacle version to clean up. By default ignores Tentacle state", v => SetFlagState(v, ref isTentacleOutdated));
        }

        protected override async Task Execute()
        {
            if (string.IsNullOrWhiteSpace(environmentName))
                throw new CommandException("Please specify an environment name using the parameter: --environment=XYZ");
            if (!healthStatuses.Any() && !statuses.Any())
                throw new CommandException("Please specify a status using the parameter: --status or --health-status");

            var environmentResource = await GetEnvironment().ConfigureAwait(false);
            IEnumerable<MachineResource> machines = await FilterByEnvironment(environmentResource).ConfigureAwait(false);
            machines = FilterByState(machines);

            CleanUpEnvironment(machines.ToList(), environmentResource);
        }

        private void CleanUpEnvironment(List<MachineResource> filteredMachines, EnvironmentResource environmentResource)
        {
            Log.Information("Found {0} machines in {1} with the status {2}", filteredMachines.Count, environmentResource.Name, GetStateFilterDescription());

            if (filteredMachines.Any(m => m.EnvironmentIds.Count > 1))
            {
                Log.Information("Note: Some of these machines belong to multiple environments. Instead of being deleted, these machines will be removed from the {0} environment.", environmentResource.Name);
            }

            foreach (var machine in filteredMachines)
            {
                // If the machine belongs to more than one environment, we should remove the machine from the environment rather than delete it altogether.
                if (machine.EnvironmentIds.Count > 1)
                {
                    Log.Information("Removing {0} {1} (ID: {2}) from {3}", machine.Name, machine.Status, machine.Id,
                        environmentResource.Name);
                    machine.EnvironmentIds.Remove(environmentResource.Id);
                    Repository.Machines.Modify(machine);
                }
                else
                {
                    Log.Information("Deleting {0} {1} (ID: {2})", machine.Name, machine.Status, machine.Id);
                    Repository.Machines.Delete(machine);
                }
            }
        }

        private IEnumerable<MachineResource> FilterByState(IEnumerable<MachineResource> environmentMachines)
        {
            var provider = new HealthStatusProvider(Repository, Log, statuses, healthStatuses);
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
            Log.Debug("Loading machines...");
            return Repository.Machines.FindMany(x =>  x.EnvironmentIds.Any(environmentId => environmentId == environmentResource.Id));
        }

        private async Task<EnvironmentResource> GetEnvironment()
        {
            Log.Debug("Loading environments...");
            var environmentResource = await Repository.Environments.FindByName(environmentName).ConfigureAwait(false);
            if (environmentResource == null)
            {
                throw new CouldNotFindException("the specified environment");
            }
            return environmentResource;
        }
    }
}
