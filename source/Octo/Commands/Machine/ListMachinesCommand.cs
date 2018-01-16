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

namespace Octopus.Cli.Commands.Machine
{
    [Command("list-machines", Description = "Lists all machines")]
    public class ListMachinesCommand : ApiCommand, ISupportFormattedOutput
    {
        readonly HashSet<string> environments = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> statuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> healthStatuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private HealthStatusProvider provider;
        List<EnvironmentResource> environmentResources;
        IEnumerable<MachineResource> environmentMachines;
        private bool? isDisabled;
        private bool? isCalamariOutdated;
        private bool? isTentacleOutdated;

        public ListMachinesCommand(IOctopusAsyncRepositoryFactory repositoryFactory, IOctopusFileSystem fileSystem, IOctopusClientFactory clientFactory, ICommandOutputProvider commandOutputProvider)
            : base(clientFactory, repositoryFactory, fileSystem, commandOutputProvider)
        {
            var options = Options.For("Listing");
            options.Add("environment=", "Name of an environment to filter by. Can be specified many times.", v => environments.Add(v));
            options.Add("status=", $"[Optional] Status of Machines filter by ({string.Join(", ", HealthStatusProvider.StatusNames)}). Can be specified many times.", v => statuses.Add(v));
            options.Add("health-status=|healthstatus=", $"[Optional] Health status of Machines filter by ({string.Join(", ", HealthStatusProvider.HealthStatusNames)}). Can be specified many times.", v => healthStatuses.Add(v));
            options.Add("disabled=", "[Optional] Disabled status filter of Machine.", v => SetFlagState(v, ref isDisabled));
            options.Add("calamari-outdated=", "[Optional] State of Calamari to filter. By default ignores Calamari state.", v => SetFlagState(v, ref isCalamariOutdated));
            options.Add("tentacle-outdated=", "[Optional] State of Tentacle version to filter. By default ignores Tentacle state", v => SetFlagState(v, ref isTentacleOutdated));
        }

        public async Task Request()
        {
            provider = new HealthStatusProvider(Repository, statuses, healthStatuses, commandOutputProvider);

            environmentResources = await GetEnvironments().ConfigureAwait(false);

            environmentMachines = await FilterByEnvironments(environmentResources).ConfigureAwait(false);
            environmentMachines = FilterByState(environmentMachines, provider);
        }

        public void PrintDefaultOutput()
        {
            LogFilteredMachines(environmentMachines, provider, environmentResources);
        }

        public void PrintJsonOutput()
        {
            commandOutputProvider.Json(environmentMachines.Select(machine => new
            {
                machine.Id,
                machine.Name,
                Status = provider.GetStatus(machine),
                Environments = machine.EnvironmentIds.Select(id => environmentResources.First(e => e.Id == id).Name)
                    .ToArray()
            }));
        }

        private void LogFilteredMachines(IEnumerable<MachineResource> environmentMachines, HealthStatusProvider provider, List<EnvironmentResource> environmentResources)
        {
            var orderedMachines = environmentMachines.OrderBy(m => m.Name).ToList();
            commandOutputProvider.Information("Machines: {Count}", orderedMachines.Count);
            foreach (var machine in orderedMachines)
            {
                commandOutputProvider.Information(" - {Machine:l} {Status:l} (ID: {MachineId:l}) in {Environments:l}", machine.Name, provider.GetStatus(machine), machine.Id,
                    string.Join(" and ", machine.EnvironmentIds.Select(id => environmentResources.First(e => e.Id == id).Name)));
            }
        }

        private Task<List<EnvironmentResource>> GetEnvironments()
        {
            commandOutputProvider.Debug("Loading environments...");
            return Repository.Environments.FindAll();
        }

        private IEnumerable<MachineResource> FilterByState(IEnumerable<MachineResource> environmentMachines, HealthStatusProvider provider)
        {
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
                environmentMachines =
                    environmentMachines.Where(
                        m =>
                            (m.Endpoint as ListeningTentacleEndpointResource)?.TentacleVersionDetails.UpgradeSuggested ==
                            isTentacleOutdated.Value);
            }
            return environmentMachines;
        }

        private  Task<List<MachineResource>> FilterByEnvironments(List<EnvironmentResource> environmentResources)
        {
            var environmentsToInclude = environmentResources.Where(e => environments.Contains(e.Name, StringComparer.OrdinalIgnoreCase)).ToList();
            var missingEnvironments = environments.Except(environmentsToInclude.Select(e => e.Name), StringComparer.OrdinalIgnoreCase).ToList();
            if (missingEnvironments.Any())
                throw new CouldNotFindException("environment(s) named", string.Join(", ", missingEnvironments));


            var environmentFilter = environmentsToInclude.Select(p => p.Id).ToList();

            commandOutputProvider.Debug("Loading machines...");
            if (environmentFilter.Count > 0)
            {
                commandOutputProvider.Debug("Loading machines from {Environments:l}...", string.Join(", ", environmentsToInclude.Select(e => e.Name)));
                return
                     Repository.Machines.FindMany(
                        x => { return x.EnvironmentIds.Any(environmentId => environmentFilter.Contains(environmentId)); });
            }
            else
            {
                commandOutputProvider.Debug("Loading machines from all environments...");
                return  Repository.Machines.FindAll();
            }
        }
    }
}
