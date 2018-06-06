using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands.WorkerPool;
using Octopus.Client.Model;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class ListWorkerPoolsCommandFixture: ApiCommandFixtureBase
    {
        ListWorkerPoolsCommand listWorkerPoolsCommand;

        [SetUp]
        public void SetUp()
        {
            listWorkerPoolsCommand = new ListWorkerPoolsCommand(RepositoryFactory, FileSystem, ClientFactory, CommandOutputProvider);
        }

        [Test]
        public async Task ShouldGetListOfWorkerPools()
        {
            SetupPools();

            await listWorkerPoolsCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain("WorkerPools: 2");
            LogLines.Should().Contain(" - default (ID: defaultid)");
            LogLines.Should().Contain(" - windows (ID: windowsid)");
        }

        [Test]
        public async Task JsonFormat_ShouldBeWellFormed()
        {
            SetupPools();
            
            CommandLineArgs.Add("--outputFormat=json");
            await listWorkerPoolsCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            var logoutput = LogOutput.ToString();
            JsonConvert.DeserializeObject(logoutput);
            logoutput.Should().Contain("defaultid");
            logoutput.Should().Contain("windowsid");
        }

        private void SetupPools()
        {
            Repository.WorkerPools.FindAll().Returns(new List<WorkerPoolResource>
            {
                new WorkerPoolResource() {Name = "default", Id = "defaultid"},
                new WorkerPoolResource() {Name = "windows", Id = "windowsid"}
            });
        }

    }
}