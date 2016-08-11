using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Cli.Infrastructure;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

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

            Log.Received().Information("Machines: 2");
            Log.Received().Information(MachineLogFormat, "PC01466", MachineModelStatus.Offline.ToString(), "Machines-002", "Development");
            Log.Received().Information(MachineLogFormat, "PC01996", MachineModelStatus.Offline.ToString(), "Machines-003", "Development");
            Log.DidNotReceive().Information(MachineLogFormat, "PC01234", MachineModelStatus.Online.ToString(), "Machines-001", "Development");
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

            Log.Received().Information("Machines: 1");
            Log.Received().Information(MachineLogFormat, "PC01234", MachineModelStatus.Online.ToString(), "Machines-001", "Development");
            Log.DidNotReceive().Information(MachineLogFormat, "PC01466", MachineModelStatus.Online.ToString(), "Machines-002", "Development");
            Log.DidNotReceive().Information(MachineLogFormat, "PC01996", MachineModelStatus.Offline.ToString(), "Machines-003", "Development");
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
            Log.Received().Information("Machines: 2");
            Log.Received().Information(MachineLogFormat, "PC01234", MachineModelStatus.Online.ToString(), "Machines-001", "Development");
            Log.Received().Information(MachineLogFormat, "PC01466", MachineModelStatus.Online.ToString(), "Machines-002", "Development");
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

            Log.Received().Information("Machines: 1");
            Log.DidNotReceive().Information(MachineLogFormat, "PC01234", MachineModelStatus.Online.ToString(), "Machines-001", "Development");
            Log.DidNotReceive().Information(MachineLogFormat, "PC01466", MachineModelStatus.Online.ToString(), "Machines-002", "Development");
            Log.Received().Information(MachineLogFormat, "PC01996", MachineModelStatus.Offline.ToString(), "Machines-003", "Development");
        }

        [Test]
        public void ShouldGetListOfMachinesWithMachineHealthStatusArgs()
        {
            CommandLineArgs.Add("--health-status=HasWarnings");
            Repository.Client.RootDocument.Version = "3.5.0";
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
                    HealthStatus = MachineModelHealthStatus.Unavailable,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                },
                new MachineResource {
                    Name = "PC01466",
                    Id = "Machines-002",
                    Status = MachineModelStatus.Online,
                    HealthStatus = MachineModelHealthStatus.HasWarnings,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                },
                new MachineResource {
                    Name = "PC01996",
                    Id = "Machines-003",
                    Status = MachineModelStatus.Offline,
                    HealthStatus = MachineModelHealthStatus.Healthy,
                    EnvironmentIds = new ReferenceCollection("Environments-001")}
            });

            listMachinesCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Information("Machines: 1");
            Log.Received().Information(MachineLogFormat, "PC01466", MachineModelHealthStatus.HasWarnings.ToString(), "Machines-002", "Development");
        }

        [Test]
        public void ShouldLogWarningIfUsingStatusOn34Repo()
        {
            CommandLineArgs.Add("--status=Online");
            Repository.Client.RootDocument.Version = "3.4.0";
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
                    HealthStatus = MachineModelHealthStatus.Unavailable,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                },
                new MachineResource {
                    Name = "PC01466",
                    Id = "Machines-002",
                    Status = MachineModelStatus.Online,
                    HealthStatus = MachineModelHealthStatus.HasWarnings,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                },
                new MachineResource {
                    Name = "PC01996",
                    Id = "Machines-003",
                    Status = MachineModelStatus.Offline,
                    HealthStatus = MachineModelHealthStatus.Healthy,
                    EnvironmentIds = new ReferenceCollection("Environments-001")}
            });

            listMachinesCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Warning("The `--status` parameter will be depricated in Octopus Deploy 4.0. You may want to execute this command with the `--health-status=` parameter instead.");
            Log.Received().Information("Machines: 2");
        }

        [Test]
        public void ShouldSupportStateFilters()
        {
            CommandLineArgs.Add("--health-status=Healthy");
            CommandLineArgs.Add("--calamari-outdated=false");
            CommandLineArgs.Add("--tentacle-outdated=true");
            CommandLineArgs.Add("--disabled=true");
            Repository.Client.RootDocument.Version = "3.4.0";
            Repository.Environments.FindAll().Returns(new List<EnvironmentResource>
            {
                new EnvironmentResource {Name = "Development", Id = "Environments-001"}
            });

            Repository.Machines.FindAll().Returns(new List<MachineResource>
            {
                new MachineResource {
                    Name = "PC0123",
                    Id = "Machines-001",
                    HealthStatus = MachineModelHealthStatus.Unavailable,
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                },
                new MachineResource {
                    Name = "PC01466",
                    Id = "Machines-002",
                    HealthStatus = MachineModelHealthStatus.Healthy,
                    IsDisabled = true,
                    HasLatestCalamari = true,
                    Endpoint = new ListeningTentacleEndpointResource() {TentacleVersionDetails = new TentacleDetailsResource { UpgradeSuggested = true } },
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                },
                new MachineResource {
                    Name = "PC01467",
                    Id = "Machines-003",
                    HealthStatus = MachineModelHealthStatus.Healthy,
                    IsDisabled = true,
                    HasLatestCalamari = true,
                    Endpoint = new ListeningTentacleEndpointResource() {TentacleVersionDetails = new TentacleDetailsResource { UpgradeSuggested = false } },
                    EnvironmentIds = new ReferenceCollection("Environments-001")
                },
                new MachineResource {
                    Name = "PC01468",
                    Id = "Machines-004",
                    HealthStatus = MachineModelHealthStatus.Healthy,
                    IsDisabled = true,
                    EnvironmentIds = new ReferenceCollection("Environments-001"),
                    HasLatestCalamari = false
                },
                new MachineResource {
                    Name = "PC01999",
                    Id = "Machines-005",
                    HealthStatus = MachineModelHealthStatus.Healthy,
                    IsDisabled = false,
                    EnvironmentIds = new ReferenceCollection("Environments-001")}
            });

            listMachinesCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Information("Machines: 1");
            Log.Received().Information(MachineLogFormat, "PC01466", "Healthy - Disabled", "Machines-002", "Development");
        }

        [Test, ExpectedException(typeof(CommandException), ExpectedMessage = "The `--health-status` parameter is only available on Octopus Server instances from 3.4.0 onwards.")]
        public void ShouldThrowIfUsingHealthStatusPre34()
        {
            CommandLineArgs.Add("--health-status=Online");
            Repository.Client.RootDocument.Version = "3.1.0";

            listMachinesCommand.Execute(CommandLineArgs.ToArray());
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

            Log.Received().Information("Machines: 2");
            Log.Received().Information(MachineLogFormat, "PC01234", MachineModelStatus.Online.ToString(), "Machines-001", "Development");
            Log.Received().Information(MachineLogFormat, "PC01466", MachineModelStatus.Online.ToString(), "Machines-002", "Development");
        }
    }
}