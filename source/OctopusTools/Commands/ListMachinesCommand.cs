using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using OctopusTools.Util;

namespace OctopusTools.Commands
{
    [Command("list-machines", Description = "Lists all machines")]
    public class ListMachinesCommand : ApiCommand
    {
        readonly HashSet<string> environments = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public ListMachinesCommand(IOctopusRepositoryFactory repositoryFactory, ILog log, IOctopusFileSystem fileSystem)
            : base(repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Listing");
            options.Add("environment=", "Name of an environment to filter by. Can be specified many times.", v => environments.Add(v));
        }

        protected override void Execute()
        {
            var machines = Repository.Machines.FindAll();

            Log.Info("Machines: " + machines.Count);

            foreach (var machine in machines)
            {
                Log.InfoFormat(" - {0} {1} (ID: {2})", machine.Name, machine.Status, machine.Id);
            }
        }
    }
}