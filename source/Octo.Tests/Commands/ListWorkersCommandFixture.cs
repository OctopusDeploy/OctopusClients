using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands.Machine;
using Octopus.Cli.Infrastructure;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class ListWorkersCommandFixture : ApiCommandFixtureBase
    {
        const string MachineLogFormat = " - {0} {1} (ID: {2}) in {3}";

        [SetUp]
        public void SetUp()
        {
            listWorkersCommand = new ListWorkersCommand(RepositoryFactory, FileSystem, ClientFactory, CommandOutputProvider);
        }

        ListWorkersCommand listWorkersCommand;

        [Test]
        public async Task ShouldGetListOfWorkersWithPoolAndStatusArgs()
        {
            CommandLineArgs.Add("-workerpool=SomePool");
            CommandLineArgs.Add("-status=Offline");

            Repository.WorkerPools.FindAll().Returns(new List<WorkerPoolResource>
            {
                new WorkerPoolResource {Name = "SomePool", Id = "WorkerPools-001"}
            });

            var workerList = MakeWorkerList(3,
                new List<MachineModelStatus>
                {
                    MachineModelStatus.Online,
                    MachineModelStatus.Offline,
                    MachineModelStatus.Offline
                }, 
                new List<ReferenceCollection>
                {
                    new ReferenceCollection("WorkerPools-001"),
                    new ReferenceCollection("WorkerPools-001"),
                    new ReferenceCollection("WorkerPools-001")
                });

            Repository.Workers.FindMany(Arg.Any<Func<WorkerResource, bool>>()).Returns(workerList);

            await listWorkersCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain("Workers: 2");
            LogLines.Should().NotContain(string.Format(MachineLogFormat, workerList[0].Name, workerList[0].Status.ToString(), workerList[0].Id, "SomePool"));
            LogLines.Should().Contain(string.Format(MachineLogFormat, workerList[1].Name, workerList[1].Status.ToString(), workerList[1].Id, "SomePool"));
            LogLines.Should().Contain(string.Format(MachineLogFormat, workerList[2].Name, workerList[2].Status.ToString(), workerList[2].Id, "SomePool"));
        }

        [Test]
        public async Task ShouldGetListOfWorkersWithPoolArgs()
        {
            CommandLineArgs.Add("-workerpool=SomePool");

            Repository.WorkerPools.FindAll().Returns(new List<WorkerPoolResource>
            {
                new WorkerPoolResource {Name = "SomePool", Id = "WorkerPools-001"}
            });

            var workerList = MakeWorkerList(1,
                new List<MachineModelStatus>
                {
                    MachineModelStatus.Online
                },
                new List<ReferenceCollection>
                {
                    new ReferenceCollection("WorkerPools-001")
                });
            Repository.Workers.FindMany(Arg.Any<Func<WorkerResource, bool>>()).Returns(workerList);

            await listWorkersCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain("Workers: 1");
            LogLines.Should().Contain(string.Format(MachineLogFormat, workerList[0].Name, workerList[0].Status.ToString(), workerList[0].Id, "SomePool"));
        }

        [Test]
        public async Task ShouldGetListOfWorkerWithNoArgs()
        {
            Repository.WorkerPools.FindAll().Returns(new List<WorkerPoolResource>
            {
                new WorkerPoolResource {Name = "SomePool", Id = "WorkerPools-001"},
                new WorkerPoolResource {Name = "SomeOtherPool", Id = "WorkerPools-002"}
            });

            var workerList = MakeWorkerList(3,
                new List<MachineModelStatus>
                {
                    MachineModelStatus.Online,
                    MachineModelStatus.Offline,
                    MachineModelStatus.Offline
                },
                new List<ReferenceCollection>
                {
                    new ReferenceCollection("WorkerPools-001"),
                    new ReferenceCollection("WorkerPools-002"),
                    new ReferenceCollection("WorkerPools-002")
                });
            Repository.Workers.FindAll().Returns(workerList);

            await listWorkersCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain("Workers: 3");
            LogLines.Should().Contain(string.Format(MachineLogFormat, workerList[0].Name, workerList[0].Status.ToString(), workerList[0].Id, "SomePool"));
            LogLines.Should().Contain(string.Format(MachineLogFormat, workerList[1].Name, workerList[1].Status.ToString(), workerList[1].Id, "SomeOtherPool"));
            LogLines.Should().Contain(string.Format(MachineLogFormat, workerList[2].Name, workerList[2].Status.ToString(), workerList[2].Id, "SomeOtherPool"));
        }

        [Test]
        public async Task ShouldGetListOfWorkersWithOfflineStatusArgs()
        {
            CommandLineArgs.Add("-status=Offline");

            Repository.WorkerPools.FindAll().Returns(new List<WorkerPoolResource>
            {
                new WorkerPoolResource {Name = "SomePool", Id = "WorkerPools-001"}
            });

            var workerList = MakeWorkerList(3,
                new List<MachineModelStatus>
                {
                    MachineModelStatus.Online,
                    MachineModelStatus.Online,
                    MachineModelStatus.Offline
                },
                new List<ReferenceCollection>
                {
                    new ReferenceCollection("WorkerPools-001"),
                    new ReferenceCollection("WorkerPools-001"),
                    new ReferenceCollection("WorkerPools-001")
                });

            Repository.Workers.FindAll().Returns(workerList);

            await listWorkersCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain("Workers: 1");
            LogLines.Should().NotContain(string.Format(MachineLogFormat, workerList[0].Name, workerList[0].Status.ToString(), workerList[0].Id, "SomePool"));
            LogLines.Should().NotContain(string.Format(MachineLogFormat, workerList[1].Name, workerList[1].Status.ToString(), workerList[1].Id, "SomePool"));
            LogLines.Should().Contain(string.Format(MachineLogFormat, workerList[2].Name, workerList[2].Status.ToString(), workerList[2].Id, "SomePool"));
        }

        [Test]
        public async Task ShouldSupportStateFilters()
        {
            CommandLineArgs.Add("--health-status=Healthy");
            CommandLineArgs.Add("--calamari-outdated=false");
            CommandLineArgs.Add("--tentacle-outdated=true");
            CommandLineArgs.Add("--disabled=true");
            Repository.Client.RootDocument.Version = "3.4.0";
            Repository.WorkerPools.FindAll().Returns(new List<WorkerPoolResource>
            {
                new WorkerPoolResource {Name = "SomePool", Id = "WorkerPools-001"}
            });

            Repository.Workers.FindAll().Returns(new List<WorkerResource>
            {
                new WorkerResource {
                    Name = "PC0123",
                    Id = "Machines-001",
                    HealthStatus = MachineModelHealthStatus.Unavailable,
                    WorkerPoolIds = new ReferenceCollection("WorkerPools-001")
                },
                new WorkerResource {
                    Name = "PC01466",
                    Id = "Machines-002",
                    HealthStatus = MachineModelHealthStatus.Healthy,
                    IsDisabled = true,
                    HasLatestCalamari = true,
                    Endpoint = new ListeningTentacleEndpointResource() {TentacleVersionDetails = new TentacleDetailsResource { UpgradeSuggested = true } },
                    WorkerPoolIds = new ReferenceCollection("WorkerPools-001")
                },
                new WorkerResource {
                    Name = "PC01467",
                    Id = "Machines-003",
                    HealthStatus = MachineModelHealthStatus.Healthy,
                    IsDisabled = true,
                    HasLatestCalamari = true,
                    Endpoint = new ListeningTentacleEndpointResource() {TentacleVersionDetails = new TentacleDetailsResource { UpgradeSuggested = false } },
                    WorkerPoolIds = new ReferenceCollection("WorkerPools-001")
                },
                new WorkerResource {
                    Name = "PC01468",
                    Id = "Machines-004",
                    HealthStatus = MachineModelHealthStatus.Healthy,
                    IsDisabled = true,
                    WorkerPoolIds = new ReferenceCollection("WorkerPools-001"),
                    HasLatestCalamari = false
                },
                new WorkerResource {
                    Name = "PC01999",
                    Id = "Machines-005",
                    HealthStatus = MachineModelHealthStatus.Healthy,
                    IsDisabled = false,
                    WorkerPoolIds = new ReferenceCollection("WorkerPools-001")}
            });

            await listWorkersCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain("Workers: 1");
            LogLines.Should().Contain(string.Format(MachineLogFormat, "PC01466", "Healthy - Disabled", "Machines-002", "SomePool"));
        }
        

        [Test]
        public async Task JsonFormat_ShouldBeWellFormed()
        {
            CommandLineArgs.Add("--outputFormat=json");
            CommandLineArgs.Add("-status=Online");

            Repository.WorkerPools.FindAll().Returns(new List<WorkerPoolResource>
            {
                new WorkerPoolResource {Name = "SomePool", Id = "WorkerPools-001"}
            });

            var workerList = MakeWorkerList(3,
                new List<MachineModelStatus>
                {
                    MachineModelStatus.Online,
                    MachineModelStatus.Online,
                    MachineModelStatus.Offline
                },
                new List<ReferenceCollection>
                {
                    new ReferenceCollection("WorkerPools-001"),
                    new ReferenceCollection("WorkerPools-001"),
                    new ReferenceCollection("WorkerPools-001")
                });

            Repository.Workers.FindAll().Returns(workerList);

            await listWorkersCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            var logoutput = LogOutput.ToString();
            JsonConvert.DeserializeObject(logoutput);
            logoutput.Should().Contain(workerList[0].Name);
            logoutput.Should().Contain(workerList[1].Name);
            logoutput.Should().NotContain(workerList[2].Name);
        }


        private List<WorkerResource> MakeWorkerList(int numWorkers, List<MachineModelStatus> statuses,
            List<ReferenceCollection> pools)
        {
            var result = new List<WorkerResource>();
            for (int i = 0; i < numWorkers; i++)
            {
                result.Add(
                    new WorkerResource
                    {
                        Name = Guid.NewGuid().ToString(),
                        Id = "Machines-00" + i,
                        Status = statuses[i],
                        WorkerPoolIds = pools[i]
                    });
            }

            return result;
        }
    }
}