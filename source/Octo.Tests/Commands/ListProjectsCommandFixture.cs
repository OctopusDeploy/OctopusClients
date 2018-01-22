using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using Octopus.Client.Model;
using FluentAssertions;
using Newtonsoft.Json;
using Octopus.Cli.Commands.Project;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class ListProjectsCommandFixture : ApiCommandFixtureBase
    {
        ListProjectsCommand listProjectsCommand;

        [SetUp]
        public void SetUp()
        {
            listProjectsCommand = new ListProjectsCommand(RepositoryFactory, FileSystem, ClientFactory, CommandOutputProvider);
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

            LogLines.Should().Contain("Projects: 2");
            LogLines.Should().Contain(" - ProjectA (ID: projectaid)");
            LogLines.Should().Contain(" - ProjectB (ID: projectbid)");
        }

        [Test]
        public async Task JsonFormat_ShouldBeWellFormed()
        {
            CommandLineArgs.Add("--outputFormat=json");
            Repository.Projects.FindAll().Returns(new List<ProjectResource>
            {
                new ProjectResource {Name = "ProjectA", Id = "projectaid"},
                new ProjectResource {Name = "ProjectB", Id = "projectbid"}
            });

            await listProjectsCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            var logoutput = LogOutput.ToString();
            JsonConvert.DeserializeObject(logoutput);
            logoutput.Should().Contain("projectaid");
            logoutput.Should().Contain("projectbid");

        }
    }
}
