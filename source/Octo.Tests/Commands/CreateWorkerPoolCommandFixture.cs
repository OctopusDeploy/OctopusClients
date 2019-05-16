using System;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands.WorkerPool;
using Octopus.Client.Model;

namespace Octo.Tests.Commands
{
    public class CreateWorkerPoolCommandFixture : ApiCommandFixtureBase
    {
        private CreateWorkerPoolCommand createWorkerPoolCommand;

        [SetUp]
        public void Setup()
        {
            createWorkerPoolCommand = new CreateWorkerPoolCommand(RepositoryFactory, FileSystem, ClientFactory, CommandOutputProvider);
        }

        [Test]
        public async Task DefaultOutput_CreateNewWorkerPool()
        {
            var newPool = Guid.NewGuid().ToString();
            CommandLineArgs.Add($"--name={newPool}");

            Repository.WorkerPools.FindByName(Arg.Any<string>()).Returns((WorkerPoolResource)null);
            Repository.WorkerPools.Create(Arg.Any<WorkerPoolResource>())
                .Returns(new WorkerPoolResource { Id = Guid.NewGuid().ToString(), Name = newPool });

            await createWorkerPoolCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain($"Creating worker pool: {newPool}");
        }

        [Test]
        public async Task JsonOutput_CreateNewWorkerPool()
        {
            var newPool = Guid.NewGuid().ToString();
            CommandLineArgs.Add($"--name={newPool}");
            CommandLineArgs.Add("--outputFormat=json");

            Repository.WorkerPools.FindByName(Arg.Any<string>()).Returns((WorkerPoolResource)null);
            Repository.WorkerPools.Create(Arg.Any<WorkerPoolResource>())
                .Returns(new WorkerPoolResource { Id = Guid.NewGuid().ToString(), Name = newPool});

            await createWorkerPoolCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            var logoutput = LogOutput.ToString();
            Console.WriteLine(logoutput);
            JsonConvert.DeserializeObject(logoutput);
            logoutput.Should().Contain(newPool);
        }

    }
}