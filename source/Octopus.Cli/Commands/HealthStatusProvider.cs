using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using Octopus.Client;
using Octopus.Client.Model;
using Serilog;

#pragma warning disable 618
namespace Octopus.Cli.Commands
{
    /// <summary>
    /// This class exists to provide backwards compataility to the pre 3.4.0 changes to machine state.
    /// As of 3.4.0 the <see cref="MachineModelStatus"/> enum has been marked as obselete to be replaced with <see cref="MachineModelHealthStatus"/>
    /// </summary>
    public class HealthStatusProvider
    {
        private readonly HashSet<string> statuses;
        private readonly HashSet<string> healthStatuses;
        private readonly ICommandOutputProvider commandOutputProvider;

        public static readonly string[] StatusNames = Enum.GetNames(typeof(MachineModelStatus));
        public static readonly string[] HealthStatusNames = Enum.GetNames(typeof(MachineModelHealthStatus));

        public HealthStatusProvider(IOctopusAsyncRepository repository, HashSet<string> statuses, HashSet<string> healthStatuses, ICommandOutputProvider commandOutputProvider)
        {
            this.statuses = statuses;
            this.healthStatuses = healthStatuses;
            this.commandOutputProvider = commandOutputProvider;
            IsHealthStatusPendingDeprication = (new SemanticVersion(repository.Client.RootDocument.Version).Version >= new SemanticVersion("3.4.0").Version);
            ValidateOptions();
        }

        void ValidateOptions()
        {
            if (IsHealthStatusPendingDeprication)
            {
                if (statuses.Any())
                {
                    commandOutputProvider.Warning("The `--status` parameter will be deprecated in Octopus Deploy 4.0. You may want to execute this command with the `--health-status=` parameter instead.");
                }
            }
            else
            {
                if (healthStatuses.Any())
                {
                    throw new CommandException("The `--health-status` parameter is only available on Octopus Server instances from 3.4.0 onwards.");
                }
            }

            var missingStatuses = statuses.Where(s => !StatusNames.Contains(s, StringComparer.OrdinalIgnoreCase)).ToList();
            if (missingStatuses.Any())
                throw new CommandException(
                    $"The following status value is unknown: {string.Join(", ", missingStatuses)}. Please choose from {string.Join(", ", StatusNames)}");


            var missingHealthStatuses = healthStatuses.Where(s => !HealthStatusNames.Contains(s, StringComparer.OrdinalIgnoreCase)).ToList();
            if (missingHealthStatuses.Any())
                throw new CommandException(
                    $"The following health status value is unknown: {string.Join(", ", missingHealthStatuses)}. Please choose from {string.Join(", ", HealthStatusNames)}");
        }

        public string GetStatus(MachineBasedResource machineResource)
        {
            if (IsHealthStatusPendingDeprication)
            {
                var status = machineResource.HealthStatus.ToString();
                if (machineResource.IsDisabled)
                    status = status + " - Disabled";
                return status;
            }

            return machineResource.Status.ToString();
        }

        private bool IsHealthStatusPendingDeprication { get; }

        public IEnumerable<TMachineResource> Filter<TMachineResource>(IEnumerable<TMachineResource> machines) where TMachineResource : MachineBasedResource
        {
            machines = FilterByProvidedStatus(machines);
            machines = FilterByProvidedHealthStatus(machines);
            return machines;
        }

        IEnumerable<TMachineResource> FilterByProvidedStatus<TMachineResource>(IEnumerable<TMachineResource> machines) where TMachineResource : MachineBasedResource
        {
            var statusFilter = new List<MachineModelStatus>();
            if (statuses.Count > 0)
            {
                commandOutputProvider.Debug("Loading statuses...");
                foreach (var status in statuses)
                {
                    MachineModelStatus result;
                    if (Enum.TryParse(status, true, out result))
                        statusFilter.Add(result);
                }
            }

            return statusFilter.Any()
                ? machines.Where(p => statusFilter.Contains(p.Status))
                : machines;
        }

        IEnumerable<TMachineResource> FilterByProvidedHealthStatus<TMachineResource>(IEnumerable<TMachineResource> machines) where TMachineResource : MachineBasedResource
        {
            var statusFilter = new List<MachineModelHealthStatus>();
            if (healthStatuses.Count > 0)
            {
                commandOutputProvider.Debug("Loading health statuses...");
                foreach (var status in healthStatuses)
                {
                    MachineModelHealthStatus result;
                    if (Enum.TryParse(status, true, out result))
                        statusFilter.Add(result);
                }
            }

            return statusFilter.Any()
                ? machines.Where(p => statusFilter.Contains(p.HealthStatus))
                : machines;
        }
    }
}