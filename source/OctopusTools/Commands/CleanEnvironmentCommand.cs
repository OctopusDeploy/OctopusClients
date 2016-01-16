using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Octopus.Client.Model;
using OctopusTools.Infrastructure;
using OctopusTools.Util;

namespace OctopusTools.Commands
{
    [Command("clean-environment", Description = "Cleans all Offline Machines from an Environment")]
    public class CleanEnvironmentCommand : ApiCommand
    {
        string environmentName;
        MachineModelStatus status = MachineModelStatus.Unknown;

        public CleanEnvironmentCommand(IOctopusRepositoryFactory repositoryFactory, ILog log, IOctopusFileSystem fileSystem)
            : base(repositoryFactory, log, fileSystem)
        {
            var options = Options.For("Cleanup");
            options.Add("environment=", "Name of an environment to clean up.", v => environmentName = v);
            options.Add("status=", "Status of Machines to clean up (Online, Offline, NeedsUpgrade, CalamariNeedsUpgrade, Disabled).", v =>
            {
                Enum.TryParse(v, true, out status);
            });
        }

        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(environmentName)) throw new CommandException("Please specify an environment name using the parameter: --environment=XYZ");
            if (status != MachineModelStatus.Unknown) throw new CommandException("Please specify status using the parameter: --status=Offline");

            Log.Debug("Loading environment...");
            var environmentResource = Repository.Environments.FindByName(environmentName);

            Log.Debug("Loading machines...");
            var machines = Repository.Machines.FindMany(x =>
            {
                return x.Status == status && x.EnvironmentIds.Any(environmentId => environmentId == environmentResource.Id);
            });

            if (machines != null)
            {
                Log.Info("Machines: " + machines.Count);

                foreach (var machine in machines)
                {
                    Log.InfoFormat(" - {0} {1} (ID: {2})", machine.Name, machine.Status, machine.Id);
                }
            }
        }
    }
}
