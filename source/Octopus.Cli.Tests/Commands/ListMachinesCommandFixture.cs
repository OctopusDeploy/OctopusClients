using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Client.Model;
#pragma warning disable 618

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class ListMachinesCommandFixture : ApiCommandFixtureBase
    {
        const string MachineLogFormat = " - {0} {1} (ID: {2}) in {3}";

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

            Repository.Environments.FindAll().Returns(new List<EnvironmentResource>
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
                    Name = "PC01996",
                    Id = "Machines-003",
                    Status = MachineModelStatus.Offline,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                },
                new MachineResource
                {
                    Name = "PC01466",
                    Id = "Machines-002",
                    Status = MachineModelStatus.Offline,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                }
            });

            listMachinesCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Info("Machines: 2");
            Log.Received().InfoFormat(MachineLogFormat, "PC01466", MachineModelStatus.Offline.ToString(), "Machines-002", "Development");
            Log.Received().InfoFormat(MachineLogFormat, "PC01996", MachineModelStatus.Offline.ToString(), "Machines-003", "Development");
            Log.DidNotReceive().InfoFormat(MachineLogFormat, "PC01234", MachineModelStatus.Online.ToString(), "Machines-001", "Development");
        }

        [Test]
        public void ShouldGetListOfMachinesWithEnvironmentArgs()
        {
            CommandLineArgs.Add("-environment=Development");

            Repository.Environments.FindAll().Returns(new List<EnvironmentResource>
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
            Log.Received().InfoFormat(MachineLogFormat, "PC01234", MachineModelStatus.Online.ToString(), "Machines-001", "Development");
            Log.DidNotReceive().InfoFormat(MachineLogFormat, "PC01466", MachineModelStatus.Online.ToString(), "Machines-002", "Development");
            Log.DidNotReceive().InfoFormat(MachineLogFormat, "PC01996", MachineModelStatus.Offline.ToString(), "Machines-003", "Development");
        }

        [Test]
        public void ShouldGetListOfMachinesWithNoArgs()
        {
            Repository.Environments.FindAll().Returns(new List<EnvironmentResource>
            {
                new EnvironmentResource {Name = "Development", Id = "Environments-001"}
            });

            Repository.Machines.FindAll().Returns(new List<MachineResource>
            {
                new MachineResource {
                    Name = "PC01234",
                    Id = "Machines-001",
                    Status = MachineModelStatus.Online,
                    HealthStatus = MachineModelHealthStatus.Healthy,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                },
                new MachineResource {
                    Name = "PC01466",
                    Id = "Machines-002",
                    Status = MachineModelStatus.Online,
                    HealthStatus = MachineModelHealthStatus.Healthy,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                }
            });

            listMachinesCommand.Execute(CommandLineArgs.ToArray());
            var calls = Log.ReceivedCalls().ToList();
            Log.Received().Info("Machines: 2");
            Log.Received().InfoFormat(MachineLogFormat, "PC01234", MachineModelStatus.Online.ToString(), "Machines-001", "Development");
            Log.Received().InfoFormat(MachineLogFormat, "PC01466", MachineModelStatus.Online.ToString(), "Machines-002", "Development");
        }

        [Test]
        public void ShouldGetListOfMachinesWithOfflineStatusArgs()
        {
            CommandLineArgs.Add("-status=Offline");

            Repository.Environments.FindAll().Returns(new List<EnvironmentResource>
            {
                new EnvironmentResource {Name = "Development", Id = "Environments-001"}
            });

            Repository.Machines.FindAll().Returns(new List<MachineResource>
            {
                new MachineResource {
                    Name = "PC0123",
                    Id = "Machines-001",
                    Status = MachineModelStatus.Online,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                },
                new MachineResource {
                    Name = "PC01466",
                    Id = "Machines-002",
                    Status = MachineModelStatus.Online,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                },
                new MachineResource {
                    Name = "PC01996",
                    Id = "Machines-003",
                    Status = MachineModelStatus.Offline,
                    EnvironmentIds = new ReferenceCollection("Environments-001")}
            });

            listMachinesCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Info("Machines: 1");
            Log.DidNotReceive().InfoFormat(MachineLogFormat, "PC01234", MachineModelStatus.Online.ToString(), "Machines-001", "Development");
            Log.DidNotReceive().InfoFormat(MachineLogFormat, "PC01466", MachineModelStatus.Online.ToString(), "Machines-002", "Development");
            Log.Received().InfoFormat(MachineLogFormat, "PC01996", MachineModelStatus.Offline.ToString(), "Machines-003", "Development");
        }

        [Test]
        public void ShouldGetListOfMachinesWithStatusArgs()
        {
            CommandLineArgs.Add("-status=Online");

            Repository.Environments.FindAll().Returns(new List<EnvironmentResource>
            {
                new EnvironmentResource {Name = "Development", Id = "Environments-001"}
            });

            Repository.Machines.FindAll().Returns(new List<MachineResource>
            {
                new MachineResource {
                    Name = "PC01234",
                    Id = "Machines-001",
                    Status = MachineModelStatus.Online,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                },
                new MachineResource {
                    Name = "PC01466",
                    Id = "Machines-002",
                    Status = MachineModelStatus.Online,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                },
                new MachineResource {
                    Name = "PC01996",
                    Id = "Machines-003",
                    Status = MachineModelStatus.Offline,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                }
            });

            listMachinesCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Info("Machines: 2");
            Log.Received().InfoFormat(MachineLogFormat, "PC01234", MachineModelStatus.Online.ToString(), "Machines-001", "Development");
            Log.Received().InfoFormat(MachineLogFormat, "PC01466", MachineModelStatus.Online.ToString(), "Machines-002", "Development");
        }
    }
}