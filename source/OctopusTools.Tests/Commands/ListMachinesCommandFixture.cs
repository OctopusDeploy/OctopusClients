using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Model;
using OctopusTools.Commands;

namespace OctopusTools.Tests.Commands
{
    [TestFixture]
    public class ListMachinesCommandFixture : ApiCommandFixtureBase
    {
        [SetUp]
        public void SetUp()
        {
            listMachinesCommand = new ListMachinesCommand(RepositoryFactory, Log, FileSystem);
        }

        ListMachinesCommand listMachinesCommand;

        [Test]
        public void ShouldGetListOfMachinesWithEnvironmentAndStatusArgs()
        {
            CommandLineArgs.Add("-environment=Development");
            CommandLineArgs.Add("-status=Offline");

            Repository.Environments.FindByNames(Arg.Any<string[]>()).Returns(new List<EnvironmentResource>
            {
                new EnvironmentResource {Name = "Development", Id = "Environments-001"}
            });

            Repository.Machines.FindMany(Arg.Any<Func<MachineResource, bool>>()).Returns(new List<MachineResource>
            {
                new MachineResource
                {
                    Name = "PC01234",
                    Id = "Machines-001",
                    Status = MachineModelStatus.Online,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                },
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
            });

            listMachinesCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Info("Machines: 2");
            Log.Received().InfoFormat(" - {0} {1} (ID: {2})", "PC01466", MachineModelStatus.Offline, "Machines-002");
            Log.Received().InfoFormat(" - {0} {1} (ID: {2})", "PC01996", MachineModelStatus.Offline, "Machines-003");
            Log.DidNotReceive().InfoFormat(" - {0} {1} (ID: {2})", "PC01234", MachineModelStatus.Online, "Machines-001");
        }

        [Test]
        public void ShouldGetListOfMachinesWithEnvironmentArgs()
        {
            CommandLineArgs.Add("-environment=Development");

            Repository.Environments.FindByNames(Arg.Any<string[]>()).Returns(new List<EnvironmentResource>
            {
                new EnvironmentResource {Name = "Development", Id = "Environments-001"}
            });

            Repository.Machines.FindMany(Arg.Any<Func<MachineResource, bool>>()).Returns(new List<MachineResource>
            {
                new MachineResource
                {
                    Name = "PC01234",
                    Id = "Machines-001",
                    Status = MachineModelStatus.Online,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                }
            });

            listMachinesCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Info("Machines: 1");
            Log.Received().InfoFormat(" - {0} {1} (ID: {2})", "PC01234", MachineModelStatus.Online, "Machines-001");
            Log.DidNotReceive().InfoFormat(" - {0} {1} (ID: {2})", "PC01466", MachineModelStatus.Online, "Machines-002");
            Log.DidNotReceive().InfoFormat(" - {0} {1} (ID: {2})", "PC01996", MachineModelStatus.Offline, "Machines-003");
        }

        [Test]
        public void ShouldGetListOfMachinesWithNoArgs()
        {
            Repository.Machines.FindAll().Returns(new List<MachineResource>
            {
                new MachineResource {Name = "PC01234", Id = "Machines-001", Status = MachineModelStatus.Online},
                new MachineResource {Name = "PC01466", Id = "Machines-002", Status = MachineModelStatus.Online}
            });

            listMachinesCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Info("Machines: 2");
            Log.Received().InfoFormat(" - {0} {1} (ID: {2})", "PC01234", MachineModelStatus.Online, "Machines-001");
            Log.Received().InfoFormat(" - {0} {1} (ID: {2})", "PC01466", MachineModelStatus.Online, "Machines-002");
        }

        [Test]
        public void ShouldGetListOfMachinesWithOfflineStatusArgs()
        {
            CommandLineArgs.Add("-status=Offline");

            Repository.Machines.FindAll().Returns(new List<MachineResource>
            {
                new MachineResource {Name = "PC01234", Id = "Machines-001", Status = MachineModelStatus.Online},
                new MachineResource {Name = "PC01466", Id = "Machines-002", Status = MachineModelStatus.Online},
                new MachineResource {Name = "PC01996", Id = "Machines-003", Status = MachineModelStatus.Offline}
            });

            listMachinesCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Info("Machines: 1");
            Log.DidNotReceive().InfoFormat(" - {0} {1} (ID: {2})", "PC01234", MachineModelStatus.Online, "Machines-001");
            Log.DidNotReceive().InfoFormat(" - {0} {1} (ID: {2})", "PC01466", MachineModelStatus.Online, "Machines-002");
            Log.Received().InfoFormat(" - {0} {1} (ID: {2})", "PC01996", MachineModelStatus.Offline, "Machines-003");
        }

        [Test]
        public void ShouldGetListOfMachinesWithStatusArgs()
        {
            CommandLineArgs.Add("-status=Online");

            Repository.Machines.FindAll().Returns(new List<MachineResource>
            {
                new MachineResource {Name = "PC01234", Id = "Machines-001", Status = MachineModelStatus.Online},
                new MachineResource {Name = "PC01466", Id = "Machines-002", Status = MachineModelStatus.Online},
                new MachineResource {Name = "PC01996", Id = "Machines-003", Status = MachineModelStatus.Offline}
            });

            listMachinesCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Info("Machines: 2");
            Log.Received().InfoFormat(" - {0} {1} (ID: {2})", "PC01234", MachineModelStatus.Online, "Machines-001");
            Log.Received().InfoFormat(" - {0} {1} (ID: {2})", "PC01466", MachineModelStatus.Online, "Machines-002");
        }
    }
}