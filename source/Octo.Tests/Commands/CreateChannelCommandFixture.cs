using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands.Channel;
using Octopus.Cli.Infrastructure;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;

namespace Octo.Tests.Commands
{
    public class CreateChannelCommandFixture : ApiCommandFixtureBase
    {
        [SetUp]
        public void SetUp()
        {
            createChannelCommand = new CreateChannelCommand(RepositoryFactory, FileSystem, ClientFactory, CommandOutputProvider);
        }

        CreateChannelCommand createChannelCommand;

        [Test]
        public void ShouldThrowBecauseOfMissingParameters()
        {
            Func<Task> exec = () => createChannelCommand.Execute(CommandLineArgs.ToArray());
            exec.ShouldThrow<CommandException>();
        }

        [Test]
        public void ShouldThrowForOlderOctopusServers()
        {
            Repository.Client.RootDocument.Returns(new RootResource
            {
                Links = new LinkCollection()//.Add("Channels", "DOES_NOT_MATTER")
            });

            CommandLineArgs.Add($"--channel={$"Channel-{Guid.NewGuid()}"}");
            CommandLineArgs.Add($"--project={$"Project-{Guid.NewGuid()}"}");
            CommandLineArgs.Add($"--lifecycle={$"Lifecycle-{Guid.NewGuid()}"}");

            Func<Task> exec = () => createChannelCommand.Execute(CommandLineArgs.ToArray());
            exec.ShouldThrow<CommandException>();
        }

        [Test]
        public async Task ShouldCreateNewChannel()
        {
            var channelName = SetupNewChannel();

            await createChannelCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain($"Channel {channelName} created");
        }

        [Test]
        public async Task ShouldUpdateExistingChannel()
        {
            var channelName = SetupExistingChannel();

            await createChannelCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            LogLines.Should().Contain($"Channel {channelName} updated");
        }

        [Test]
        public async Task JsonFormat_NewChannel()
        {
            var channelName = SetupNewChannel();
            CommandLineArgs.Add("--outputFormat=json");
            await createChannelCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            var logoutput = LogOutput.ToString();
            JsonConvert.DeserializeObject(logoutput);
            logoutput.Should().Contain(channelName);
            logoutput.Should().Contain("Created");
        }

        [Test]
        public async Task JsonFormat_UpdatedChannel()
        {
            var channelName = SetupExistingChannel();
            CommandLineArgs.Add("--outputFormat=json");
            await createChannelCommand.Execute(CommandLineArgs.ToArray()).ConfigureAwait(false);

            var logoutput = LogOutput.ToString();
            JsonConvert.DeserializeObject(logoutput);
            logoutput.Should().Contain(channelName);
            logoutput.Should().Contain("Updated");
        }
        
        private string SetupExistingChannel()
        {
            Repository.Client.RootDocument.Returns(new RootResource
            {
                Links = new LinkCollection().Add("Channels", "DOES_NOT_MATTER")
            });

            var projectName = $"Project-{Guid.NewGuid()}";
            var project = new ProjectResource()
            {
                Links = new LinkCollection()
            };
            project.Links.Add("Channels", "DOES_NOT_MATTER");
            Repository.Projects.FindByName(projectName).Returns(project);

            var lifecycleName = $"Lifecycle-{Guid.NewGuid()}";
            Repository.Lifecycles.FindOne(Arg.Any<Func<LifecycleResource, bool>>())
                .Returns(new LifecycleResource {Id = lifecycleName});

            var channelName = $"Channel-{Guid.NewGuid()}";
            var channel = new ChannelResource()
            {
                Id = Guid.NewGuid().ToString(),
                Name = channelName
            };

            Repository.Projects.GetChannels(Arg.Any<ProjectResource>())
                .Returns(new ResourceCollection<ChannelResource>(new[] {channel}, new LinkCollection()));

            CommandLineArgs.Add($"--channel={channelName}");
            CommandLineArgs.Add($"--project={projectName}");
            CommandLineArgs.Add($"--lifecycle={lifecycleName}");
            CommandLineArgs.Add("--update-existing");
            return channelName;
        }

        private string SetupNewChannel()
        {
            Repository.Client.RootDocument.Returns(new RootResource
            {
                Links = new LinkCollection().Add("Channels", "DOES_NOT_MATTER")
            });

            var projectName = $"Project-{Guid.NewGuid()}";
            var project = new ProjectResource()
            {
                Links = new LinkCollection().Add("Channels", "DOES_NOT_MATTER")
            };
            Repository.Projects.FindByName(projectName).Returns(project);

            var lifecycleName = $"Lifecycle-{Guid.NewGuid()}";
            Repository.Lifecycles.FindOne(Arg.Any<Func<LifecycleResource, bool>>()).Returns(new LifecycleResource());

            Repository.Projects.GetChannels(Arg.Any<ProjectResource>())
                .Returns(new ResourceCollection<ChannelResource>(Enumerable.Empty<ChannelResource>(), new LinkCollection()));

            var channelName = $"Channel-{Guid.NewGuid()}";
            CommandLineArgs.Add($"--channel={channelName}");
            CommandLineArgs.Add($"--project={projectName}");
            CommandLineArgs.Add($"--lifecycle={lifecycleName}");
            return channelName;
        }
    }
}