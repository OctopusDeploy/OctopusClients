using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Octopus.Client.Model;
using OctopusTools.Util;

namespace OctopusTools.Commands
{
    [Command("list-machines", Description = "Lists all machines")]
    public class ListMachinesCommand : ApiCommand
    {
        readonly HashSet<string> environments = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        readonly HashSet<string> statuses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public ListMachinesCommand(IOctopusRepositoryFactory repositoryFactory, ILog log, IOctopusFileSystem fileSystem)
            : base(repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Listing");
            options.Add("environment=", "Name of an environment to filter by. Can be specified many times.", v => environments.Add(v));
            options.Add("status=", "Status of Machines filter by (Online, Offline, Unknown, NeedsUpgrade, CalamariNeedsUpgrade, Disabled). Can be specified many times.", v => statuses.Add(v));
        }

        protected override void Execute()
        {
            var machines = new List<MachineResource>();
            var filteredMachines = new List<MachineResource>();
            var environmentFilter = new List<string>();
            var statusFilter = new List<MachineModelStatus>();

            if (environments.Count > 0)
            {
                Log.Debug("Loading environments...");
                var environmentResources = Repository.Environments.FindByNames(environments.ToArray());
                environmentFilter = environmentResources.Select(p => p.Id).ToList();
            }

            if (statuses.Count > 0)
            {
                Log.Debug("Loading statuses...");
                foreach (var status in statuses)
                {
                    MachineModelStatus result;
                    if (Enum.TryParse(status, true, out result))
                        statusFilter.Add(result);
                }
            }

            Log.Debug("Loading machines...");
            if (environmentFilter.Count > 0)
                machines = Repository.Machines.FindMany(x => { return x.EnvironmentIds.Any(environmentId => environmentFilter.Contains(environmentId)); });
            else
                machines = Repository.Machines.FindAll();

            if (machines != null)
                filteredMachines.AddRange(machines);

            var machinesStatus = filteredMachines.Where(p => !statusFilter.Any() || statusFilter.Contains(p.Status)).ToList();
            Log.Info("Machines: " + machinesStatus.Count);

            foreach (var machine in machinesStatus)
            {
                Log.InfoFormat(" - {0} {1} (ID: {2})", machine.Name, machine.Status, machine.Id);
            }
        }
    }
}