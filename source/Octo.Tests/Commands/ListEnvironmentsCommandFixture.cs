using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Client.Model;
using FluentAssertions;
using Newtonsoft.Json;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class ListEnvironmentsCommandFixture: ApiCommandFixtureBase
    {
        ListEnvironmentsCommand listEnvironmentsCommand;

        [SetUp]
        public void SetUp()
        {
            listEnvironmentsCommand = new ListEnvironmentsCommand(RepositoryFactory, Log, FileSystem, ClientFactory, CommandOutputProvider);
        }

        [Test]
        public async Task ShouldGetListOfEnvironments()
        {
            SetupEnvironments();

            await listEnvironmentsCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain("Environments: 2");
            LogLines.Should().Contain(" - Dev (ID: devenvid)");
            LogLines.Should().Contain(" - Prod (ID: prodenvid)");
        }

        [Test]
        public async Task JsonFormat_ShouldBeWellFormed()
        {
            SetupEnvironments();
            
            CommandLineArgs.Add("--output=json");
            await listEnvironmentsCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            var logoutput = LogOutput.ToString();
            JsonConvert.DeserializeObject(logoutput);
            logoutput.Should().Contain("devenvid");
            logoutput.Should().Contain("prodenvid");
        }

        private void SetupEnvironments()
        {
            Repository.Environments.FindAll().Returns(new List<EnvironmentResource>
            {
                new EnvironmentResource() {Name = "Dev", Id = "devenvid"},
                new EnvironmentResource() {Name = "Prod", Id = "prodenvid"}
            });
        }

    }
}