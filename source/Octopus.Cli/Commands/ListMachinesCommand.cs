using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    [Command("list-machines", Description = "Lists all machines")]
    public class ListMachinesCommand : ApiCommand
    {
        readonly HashSet<string> environments = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> statuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        string[] StatusNames = Enum.GetNames(typeof (MachineModelHealthStatus));

        public ListMachinesCommand(IOctopusRepositoryFactory repositoryFactory, ILog log, IOctopusFileSystem fileSystem)
            : base(repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Listing");
            options.Add("environment=", "Name of an environment to filter by. Can be specified many times.", v => environments.Add(v));
            options.Add("status=", string.Format("Status of Machines filter by ({0}). Can be specified many times.", string.Join(", ", StatusNames)), v => statuses.Add(v));
        }

        protected override void Execute()
        {
            var missingStatuses = statuses.Where(s => !StatusNames.Contains(s, StringComparer.OrdinalIgnoreCase)).ToList();
            if (missingStatuses.Any())
                throw new CommandException(string.Format("The following status value is unknown: {0}. Please choose from {1}",
                    string.Join(", ", missingStatuses), string.Join(", ", StatusNames)));

            var statusFilter = new List<MachineModelHealthStatus>();

            Log.Debug("Loading environments...");
            var environmentResources = Repository.Environments.FindAll();
            var environmentsToInclude = environmentResources.Where(e => environments.Contains(e.Name, StringComparer.OrdinalIgnoreCase)).ToList();
            var missingEnvironments = environments.Except(environmentsToInclude.Select(e => e.Name), StringComparer.OrdinalIgnoreCase).ToList();
            if (missingEnvironments.Any())
                throw new CouldNotFindException("environment(s) named", string.Join(", ", missingEnvironments));

            var environmentFilter = environmentsToInclude.Select(p => p.Id).ToList();

            Log.Debug("Loading machines...");
            List<MachineResource> environmentMachines;
            if (environmentFilter.Count > 0)
            {
                Log.DebugFormat("Loading machines from {0}...", string.Join(", ", environmentsToInclude.Select(e => e.Name)));
                environmentMachines = Repository.Machines.FindMany(x => { return x.EnvironmentIds.Any(environmentId => environmentFilter.Contains(environmentId)); });
            }
            else
            {
                Log.Debug("Loading machines from all environments...");
                environmentMachines = Repository.Machines.FindAll();
            }

            if (statuses.Count > 0)
            {
                Log.Debug("Loading statuses...");
                foreach (var status in statuses)
                {
                    MachineModelHealthStatus result;
                    if (Enum.TryParse(status, true, out result))
                        statusFilter.Add(result);
                }
            }

            var machines = statusFilter.Any()
                ? environmentMachines.Where(p => statusFilter.Contains(p.HealthStatus))
                : environmentMachines;

            var orderedMachines = machines.OrderBy(m => m.Name).ToList();

            Log.Info("Machines: " + orderedMachines.Count);

            foreach (var machine in orderedMachines)
            {
                Log.InfoFormat(" - {0} {1} (ID: {2}) in {3}", machine.Name, machine.HealthStatus, machine.Id, string.Join(" and ", machine.EnvironmentIds.Select(id => environmentResources.First(e => e.Id == id).Name)));
            }
        }
    }
}