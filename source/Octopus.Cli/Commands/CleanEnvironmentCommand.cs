using System;
using System.Linq;
using log4net;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    [Command("clean-environment", Description = "Cleans all Offline Machines from an Environment")]
    public class CleanEnvironmentCommand : ApiCommand
    {
        string environmentName;
        MachineModelHealthStatus status = MachineModelHealthStatus.Unknown;

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
            if (status == MachineModelHealthStatus.Unknown) throw new CommandException("Please specify a status using the parameter: --status=Offline");

            Log.Debug("Loading environment...");
            var environmentResource = Repository.Environments.FindByName(environmentName);
            if (environmentResource == null)
            {
                throw new CouldNotFindException("the specified environment");
            }

            Log.Debug("Loading machines...");
            var machines = Repository.Machines.FindMany(x =>
            {
                return x.HealthStatus == status && x.EnvironmentIds.Any(environmentId => environmentId == environmentResource.Id);
            });

            if (machines != null)
            {
                Log.InfoFormat("Found {0} machines in {1} with the status {2}", machines.Count, environmentResource.Name, status);

                if (machines.Any(m => m.EnvironmentIds.Count > 1))
                {
                    Log.InfoFormat("Note: Some of these machines belong to multiple environments. Instead of being deleted, these machines will be removed from the {0} environment.", environmentResource.Name);
                }

                foreach (var machine in machines)
                {
                    // If the machine belongs to more than one environment, we should remove the machine from the environment rather than delete it altogether.
                    if (machine.EnvironmentIds.Count > 1)
                    {
                        Log.InfoFormat("Removing {0} {1} (ID: {2}) from {3}", machine.Name, machine.HealthStatus, machine.Id, environmentResource.Name);
                        machine.EnvironmentIds.Remove(environmentResource.Id);
                        Repository.Machines.Modify(machine);
                    }
                    else
                    {
                        Log.InfoFormat("Deleting {0} {1} (ID: {2})", machine.Name, machine.HealthStatus, machine.Id);
                        Repository.Machines.Delete(machine);
                    }
                }
            }
        }
    }
}
