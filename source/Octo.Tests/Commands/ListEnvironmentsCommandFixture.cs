using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Client.Model;
using FluentAssertions;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class ListEnvironmentsCommandFixture: ApiCommandFixtureBase
    {
        ListEnvironmentsCommand listEnvironmentsCommand;

        [SetUp]
        public void SetUp()
        {
            listEnvironmentsCommand = new ListEnvironmentsCommand(RepositoryFactory, Log, FileSystem, ClientFactory);
        }

        [Test]
        public async Task ShouldGetListOfEnvironments()
        {
            Repository.Environments.FindAll().Returns(new List<EnvironmentResource>
            {
                new EnvironmentResource() {Name = "Dev", Id = "devenvid"},
                new EnvironmentResource() {Name = "Prod", Id = "prodenvid"}
            });

            await listEnvironmentsCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain("[Information] Environments: 2");
            LogLines.Should().Contain("[Information]  - Dev (ID: devenvid)");
            LogLines.Should().Contain("[Information]  - Prod (ID: prodenvid)");
        }
    }
}