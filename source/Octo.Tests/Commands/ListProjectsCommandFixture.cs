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
    public class ListProjectsCommandFixture : ApiCommandFixtureBase
    {
        ListProjectsCommand listProjectsCommand;

        [SetUp]
        public void SetUp()
        {
            listProjectsCommand = new ListProjectsCommand(RepositoryFactory, Log, FileSystem, ClientFactory);
        }

        [Test]
        public async Task ShouldGetListOfProjects()
        {
            Repository.Projects.FindAll().Returns(new List<ProjectResource>
            {
                new ProjectResource {Name = "ProjectA", Id = "projectaid"},
                new ProjectResource {Name = "ProjectB", Id = "projectbid"}
            });

            await listProjectsCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain("[Information] Projects: 2");
            LogLines.Should().Contain("[Information]  - ProjectA (ID: projectaid)");
            LogLines.Should().Contain("[Information]  - ProjectB (ID: projectbid)");
        }
    }
}
