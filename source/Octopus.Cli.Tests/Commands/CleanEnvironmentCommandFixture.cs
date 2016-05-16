using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Cli.Infrastructure;
using Octopus.Client.Model;

namespace Octopus.Cli.Tests.Commands
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

            Log.Received().InfoFormat("Found {0} machines in {1} with the status {2}", machineList.Count, "Development", MachineModelStatus.Offline);

            Log.Received().InfoFormat("Deleting {0} {1} (ID: {2})", machineList[0].Name, machineList[0].Status, machineList[0].Id);
            Repository.Machines.Received().Delete(machineList[0]);

            Log.Received().InfoFormat("Deleting {0} {1} (ID: {2})", machineList[1].Name, machineList[1].Status, machineList[1].Id);
            Repository.Machines.Received().Delete(machineList[1]);
        }

        [Test]
        public void ShouldRemoveMachinesBelongingToMultipleEnvironmentsInsteadOfDeleting()
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
                    EnvironmentIds = new ReferenceCollection(new[] {"Environments-001", "Environments-002"})
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

            Log.Received().InfoFormat("Found {0} machines in {1} with the status {2}", machineList.Count, "Development", MachineModelStatus.Offline);
            Log.Received().InfoFormat("Note: Some of these machines belong to multiple environments. Instead of being deleted, these machines will be removed from the {0} environment.", "Development");

            Log.Received().InfoFormat("Removing {0} {1} (ID: {2}) from {3}", machineList[0].Name, machineList[0].Status, machineList[0].Id, "Development");
            Assert.That(machineList[0].EnvironmentIds.Count, Is.EqualTo(1), "The machine should have been removed from the Development environment.");
            Repository.Machines.Received().Modify(machineList[0]);

            Log.Received().InfoFormat("Deleting {0} {1} (ID: {2})", machineList[1].Name, machineList[1].Status, machineList[1].Id);
            Repository.Machines.Received().Delete(machineList[1]);
        }

        [Test, ExpectedException(typeof(CommandException), ExpectedMessage = "Please specify an environment name using the parameter: --environment=XYZ")]
        public void ShouldNotCleanEnvironmentWithMissingEnvironmentArgs()
        {
            listMachinesCommand.Execute(CommandLineArgs.ToArray());
        }

        [Test, ExpectedException(typeof(CommandException), ExpectedMessage = "Please specify a status using the parameter: --status=Offline")]
        public void ShouldNotCleanEnvironmentWithMissingStatusArgs()
        {
            CommandLineArgs.Add("-environment=Development");
            listMachinesCommand.Execute(CommandLineArgs.ToArray());
        }

        [Test, ExpectedException(typeof(CouldNotFindException), ExpectedMessage = "Could not find the specified environment; either it does not exist or you lack permissions to view it.")]
        public void ShouldNotCleanTheEnvironmentIfEnvironmentNotFound()
        {
            CommandLineArgs.Add("-environment=Development");
            CommandLineArgs.Add("-status=Offline");

            listMachinesCommand.Execute(CommandLineArgs.ToArray());
        }
    }
}
