using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Client.Model;

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
        public void ShouldGetListOfEnvironments()
        {
            Repository.Environments.FindAll().Returns(new List<EnvironmentResource>
            {
                new EnvironmentResource() {Name = "Dev", Id = "devenvid"},
                new EnvironmentResource() {Name = "Prod", Id = "prodenvid"}
            });

            listEnvironmentsCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Information("Environments: 2");
            Log.Received().Information(" - {0} (ID: {1})", "Dev", "devenvid");
            Log.Received().Information(" - {0} (ID: {1})", "Prod", "prodenvid");
        }
    }
}