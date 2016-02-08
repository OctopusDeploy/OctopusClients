using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Model;
using OctopusTools.Commands;

namespace OctopusTools.Tests.Commands
{
    [TestFixture]
    public class ListEnvironmentsCommandFixture: ApiCommandFixtureBase
    {
        ListEnvironmentsCommand listEnvironmentsCommand;

        [SetUp]
        public void SetUp()
        {
            listEnvironmentsCommand = new ListEnvironmentsCommand(RepositoryFactory, Log, FileSystem);
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

            Log.Received().Info("Environments: 2");
            Log.Received().InfoFormat(" - {0} (ID: {1})", "Dev", "devenvid");
            Log.Received().InfoFormat(" - {0} (ID: {1})", "Prod", "prodenvid");
        }
    }
}