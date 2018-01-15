using System;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands.Project;
using Octopus.Cli.Tests.Commands;
using Octopus.Client.Model;

namespace Octo.Tests.Commands
{
    public class CreateProjectCommandFixture : ApiCommandFixtureBase
    {
        private CreateProjectCommand createProjectCommand;
        private string projectName, groupName, lifecycleName, projectId;

        [SetUp]
        public void Setup()
        {
            createProjectCommand = new CreateProjectCommand(RepositoryFactory, FileSystem, ClientFactory,
                CommandOutputProvider);

            projectName = Guid.NewGuid().ToString();
            groupName = Guid.NewGuid().ToString();
            lifecycleName = Guid.NewGuid().ToString();
            projectId = Guid.NewGuid().ToString();
            CommandLineArgs.AddRange(new[]
            {
                $"--name={projectName}",
                $"--projectGroup={groupName}",
                $"--lifecycle={lifecycleName}"
            });
        }

        [Test]
        public async Task DefaultOutput_ShouldCreateNewProject()
        {
            Repository.ProjectGroups.Create(Arg.Any<ProjectGroupResource>())
                .Returns(new ProjectGroupResource { Id = Guid.NewGuid().ToString(), Name = groupName });
            Repository.Lifecycles.FindOne(Arg.Any<Func<LifecycleResource, bool>>())
                .Returns(new LifecycleResource { Id = Guid.NewGuid().ToString(), Name = lifecycleName });
            Repository.Projects.Create(Arg.Any<ProjectResource>())
                .Returns(new ProjectResource { Id = projectId, Name = projectName });

            await createProjectCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain($"Creating project: {projectName}");
            LogLines.Should().Contain($"Project created. ID: {projectId}");
            LogLines.Should().Contain("Project group does not exist, it will be created");
        }

        [Test]
        public async Task JsonOutput_ShouldCreateNewProjectAndNewGroup()
        {
            CommandLineArgs.Add("--outputFormat=json");
            Repository.ProjectGroups.Create(Arg.Any<ProjectGroupResource>())
                .Returns(new ProjectGroupResource { Id = Guid.NewGuid().ToString(), Name = groupName });
            Repository.Lifecycles.FindOne(Arg.Any<Func<LifecycleResource, bool>>())
                .Returns(new LifecycleResource { Id = Guid.NewGuid().ToString(), Name = lifecycleName });
            Repository.Projects.Create(Arg.Any<ProjectResource>())
                .Returns(new ProjectResource { Id = projectId, Name = projectName });

            await createProjectCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            string logoutput = LogOutput.ToString();
            JsonConvert.DeserializeObject(logoutput);
            logoutput.Should().Contain(projectId);
            logoutput.Should().Contain(projectName);
            logoutput.Should().Contain(groupName);
            logoutput
                .Replace(Environment.NewLine, String.Empty)
                .Replace(" ", string.Empty)
                .Replace("\"", string.Empty).Should().Contain("NewGroupCreated:true");
        }

        [Test]
        public async Task JsonOutput_ShouldCreateNewProject()
        {
            CommandLineArgs.Add("--outputFormat=json");
            Repository.ProjectGroups.FindByName(Arg.Any<string>()).Returns(new ProjectGroupResource {Name = groupName});
            Repository.Lifecycles.FindOne(Arg.Any<Func<LifecycleResource, bool>>())
                .Returns(new LifecycleResource { Id = Guid.NewGuid().ToString(), Name = lifecycleName });
            Repository.Projects.Create(Arg.Any<ProjectResource>())
                .Returns(new ProjectResource { Id = projectId, Name = projectName });

            await createProjectCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            string logoutput = LogOutput.ToString();
            JsonConvert.DeserializeObject(logoutput);
            logoutput.Should().Contain(projectId);
            logoutput.Should().Contain(projectName);
            logoutput.Should().Contain(groupName);
            logoutput
                .Replace(Environment.NewLine, String.Empty)
                .Replace(" ", string.Empty)
                .Replace("\"", string.Empty)
                .Should().Contain("NewGroupCreated:false");
        }



    }
}
