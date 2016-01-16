using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Model;
using OctopusTools.Commands;
using OctopusTools.Infrastructure;

namespace OctopusTools.Tests.Commands
{
    [TestFixture]
    public class CleanEnvironmentCommandFixture : ApiCommandFixtureBase
    {
        [SetUp]
        public void SetUp()
        {
            listMachinesCommand = new CleanEnvironmentCommand(RepositoryFactory, Log, FileSystem);
        }

        CleanEnvironmentCommand listMachinesCommand;

        [Test]
        public void ShouldCleanEnvironmentWithEnvironmentAndStatusArgs()
        {
            CommandLineArgs.Add("-environment=Development");
            CommandLineArgs.Add("-status=Offline");

            Repository.Environments.FindByName("Development").Returns(
                new EnvironmentResource {Name = "Development", Id = "Environments-001"}
            );

            var machineList = new List<MachineResource>
            {
                new MachineResource
                {
                    Name = "PC01466",
                    Id = "Machines-002",
                    Status = MachineModelStatus.Offline,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                },
                new MachineResource
                {
                    Name = "PC01996",
                    Id = "Machines-003",
                    Status = MachineModelStatus.Offline,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                }
            };

            Repository.Machines.FindMany(Arg.Any<Func<MachineResource, bool>>()).Returns(machineList);

            listMachinesCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Info("Machines: 2");

            Log.Received().InfoFormat("Deleting - {0} {1} (ID: {2})", machineList[0].Name, machineList[0].Status, machineList[0].Id);
            Repository.Machines.Received().Delete(machineList[0]);

            Log.Received().InfoFormat("Deleting - {0} {1} (ID: {2})", machineList[1].Name, machineList[1].Status, machineList[1].Id);
            Repository.Machines.Received().Delete(machineList[1]);
        }

        [Test, ExpectedException(typeof(CommandException), ExpectedMessage = "Please specify an environment name using the parameter: --environment=XYZ")]
        public void ShouldNotCleanEnvironmentWithMissingEnvironmentArgs()
        {
            listMachinesCommand.Execute(CommandLineArgs.ToArray());
        }

        [Test, ExpectedException(typeof(CommandException), ExpectedMessage = "Please specify status using the parameter: --status=Offline")]
        public void ShouldNotCleanEnvironmentWithMissingStatusArgs()
        {
            CommandLineArgs.Add("-environment=Development");
            listMachinesCommand.Execute(CommandLineArgs.ToArray());
        }
    }
}
