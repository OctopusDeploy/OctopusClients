using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Client.Model;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class ListProjectsCommandFixture : ApiCommandFixtureBase
    {
        ListProjectsCommand listProjectsCommand;

        [SetUp]
        public void SetUp()
        {
            listProjectsCommand = new ListProjectsCommand(RepositoryFactory, Log, FileSystem);
        }

        [Test]
        public void ShouldGetListOfProjects()
        {
            Repository.Projects.FindAll().Returns(new List<ProjectResource>
            {
                new ProjectResource {Name = "ProjectA", Id = "projectaid"},
                new ProjectResource {Name = "ProjectB", Id = "projectbid"}
            });

            listProjectsCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Info("Projects: 2");
            Log.Received().InfoFormat(" - {0} (ID: {1})", "ProjectA", "projectaid");
            Log.Received().InfoFormat(" - {0} (ID: {1})", "ProjectB", "projectbid");
        }
    }
}
