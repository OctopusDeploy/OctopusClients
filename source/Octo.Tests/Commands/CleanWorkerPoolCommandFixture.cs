using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands.WorkerPools;
using Octopus.Cli.Infrastructure;
using Octopus.Client.Model;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class CleanWorkerPoolCommandFixture : ApiCommandFixtureBase
    {
        [SetUp]
        public void SetUp()
        {
            cleanPoolCommand = new CleanWorkerPoolCommand(RepositoryFactory, FileSystem, ClientFactory, CommandOutputProvider);
        }

        CleanWorkerPoolCommand cleanPoolCommand;

        [Test]
        public void ShouldCleanPool()
        {
            CommandLineArgs.Add("-workerpool=SomePool");
            CommandLineArgs.Add("-status=Offline");

            Repository.WorkerPools.FindByName("SomePool").Returns(
                new WorkerPoolResource {Name = "SomePool", Id = "WorkerPools-001"}
            );

            var workerList = MakeWorkerMachineList(2,
                new List<ReferenceCollection>
                {
                    new ReferenceCollection("WorkerPools-001"),
                    new ReferenceCollection("WorkerPools-001")
                });

            Repository.WorkerMachines.FindMany(Arg.Any<Func<WorkerMachineResource, bool>>()).Returns(workerList);

            cleanPoolCommand.Execute(CommandLineArgs.ToArray()).GetAwaiter().GetResult();

            LogLines.Should().Contain(string.Format("Found {0} machines in {1} with the status {2}", workerList.Count, "SomePool", MachineModelStatus.Offline.ToString()));

            LogLines.Should().Contain(string.Format("Deleting {0} {1} (ID: {2})", workerList[0].Name, workerList[0].Status, workerList[0].Id));
            Repository.WorkerMachines.Received().Delete(workerList[0]);

            LogLines.Should().Contain(string.Format("Deleting {0} {1} (ID: {2})", workerList[1].Name, workerList[1].Status, workerList[1].Id));
            Repository.WorkerMachines.Received().Delete(workerList[1]);
        }

        [Test]
        public void ShouldRemoveMachinesBelongingToMultiplePoolsInsteadOfDeleting()
        {
            CommandLineArgs.Add("-workerpool=SomePool");
            CommandLineArgs.Add("-status=Offline");

            Repository.WorkerPools.FindByName("SomePool").Returns(
                new WorkerPoolResource { Name = "SomePool", Id = "WorkerPools-001"}
            );

            var workerList = MakeWorkerMachineList(2,
                new List<ReferenceCollection>
                {
                    new ReferenceCollection(new List<string> { "WorkerPools-001", "WorkerPools-002" }),
                    new ReferenceCollection("WorkerPools-001")
                });

            Repository.WorkerMachines.FindMany(Arg.Any<Func<WorkerMachineResource, bool>>()).Returns(workerList);

            cleanPoolCommand.Execute(CommandLineArgs.ToArray()).GetAwaiter().GetResult();

            LogLines.Should().Contain(string.Format("Found {0} machines in {1} with the status {2}", workerList.Count, "SomePool", MachineModelStatus.Offline.ToString()));
            LogLines.Should().Contain("Note: Some of these machines belong to multiple pools. Instead of being deleted, these machines will be removed from the SomePool pool.");

            LogLines.Should().Contain($"Removing {workerList[0].Name} {workerList[0].Status} (ID: {workerList[0].Id}) from SomePool");
            Assert.That(workerList[0].WorkerPoolIds.Count, Is.EqualTo(1), "The machine should have been removed from the SomePool pool.");
            Repository.WorkerMachines.Received().Modify(workerList[0]);

            LogLines.Should().Contain(string.Format("Deleting {0} {1} (ID: {2})", workerList[1].Name, workerList[1].Status, workerList[1].Id));
            Repository.WorkerMachines.Received().Delete(workerList[1]);
        }

        [Test]
        public void ShouldNotCleanPoolWithMissingPoolArgs()
        {
            Func<Task> exec = () => cleanPoolCommand.Execute(CommandLineArgs.ToArray());
            exec.ShouldThrow<CommandException>()
                .WithMessage("Please specify a worker pool name using the parameter: --workerpool=XYZ");
        }

        [Test]
        public void ShouldNotCleanPoolWithMissingStatusArgs()
        {
            CommandLineArgs.Add("-workerpool=SomePool");
            Func<Task> exec = () => cleanPoolCommand.Execute(CommandLineArgs.ToArray());
            exec.ShouldThrow<CommandException>()
                .WithMessage("Please specify a status using the parameter: --health-status");
        }

        [Test]
        public void ShouldNotCleanIfPoolNotFound()
        {
            CommandLineArgs.Add("-workerpool=SomePool");
            CommandLineArgs.Add("-status=Offline");

            Func<Task> exec = () => cleanPoolCommand.Execute(CommandLineArgs.ToArray());
            exec.ShouldThrow<CouldNotFindException>()
                .WithMessage("Could not find the specified worker pool; either it does not exist or you lack permissions to view it.");
        }


        [Test]
        public async Task JsonOutput_ShouldBeWellFormed()
        {
            Repository.WorkerPools.FindByName("SomePool").Returns(
                new WorkerPoolResource { Name = "SomePool", Id = "WorkerPools-001" });

            CommandLineArgs.Add("--outputFormat=json");
            CommandLineArgs.Add($"--workerpool=SomePool");
            CommandLineArgs.Add("-status=Offline");

            var workerList = MakeWorkerMachineList(2,
                new List<ReferenceCollection>
                {
                    new ReferenceCollection(new List<string> { "WorkerPools-001", "WorkerPools-002" }),
                    new ReferenceCollection("WorkerPools-001")
                });    

            Repository.WorkerMachines.FindMany(Arg.Any<Func<WorkerMachineResource, bool>>()).Returns(workerList);

            await cleanPoolCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            string logoutput = LogOutput.ToString();
            Console.WriteLine(logoutput);
            JsonConvert.DeserializeObject(logoutput);
            Regex.Matches(logoutput, CleanWorkerPoolCommand.MachineAction.Deleted.ToString()).Count.Should()
                .Be(1, "should only have one deleted machine");
            Regex.Matches(logoutput, CleanWorkerPoolCommand.MachineAction.RemovedFromPool.ToString()).Count.Should()
                .Be(1, "should only have one machine removed from the environment");
            logoutput.Should().Contain(workerList[0].Name);
            logoutput.Should().Contain(workerList[0].Id);
            logoutput.Should().Contain(workerList[1].Name);
            logoutput.Should().Contain(workerList[1].Id);
        }
     

        private List<WorkerMachineResource> MakeWorkerMachineList(int numWorkers, List<ReferenceCollection> pools)
        {
            var result = new List<WorkerMachineResource>();
            for (int i = 0; i < numWorkers; i++)
            {
                result.Add(
                    new WorkerMachineResource
                    {
                        Name = Guid.NewGuid().ToString(),
                        Id = "Machines-00" + i,
                        Status = MachineModelStatus.Offline,
                        WorkerPoolIds = pools[i]
                    });
            }

            return result;
        }
    }
}