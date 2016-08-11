using System;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Cli.Infrastructure;
using Octopus.Client.Model;

namespace Octopus.Cli.Tests.Commands
{
    public class CreateChannelCommandFixture : ApiCommandFixtureBase
    {
        [SetUp]
        public void SetUp()
        {
            createChannelCommand = new CreateChannelCommand(RepositoryFactory, Log, FileSystem);
        }

        CreateChannelCommand createChannelCommand;

        [Test]
        public void ShouldThrowBecauseOfMissingParameters()
        {
            Assert.Throws<CommandException>(() => createChannelCommand.Execute(CommandLineArgs.ToArray()));
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

            Assert.Throws<CommandException>(() => createChannelCommand.Execute(CommandLineArgs.ToArray()));
        }

        [Test]
        public void ShouldCreateNewChannel()
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

            createChannelCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Information("Channel {0} created", channelName);
        }

        [Test]
        public void ShouldUpdateExistingChannel()
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
            Repository.Lifecycles.FindOne(Arg.Any<Func<LifecycleResource, bool>>()).Returns(new LifecycleResource { Id = lifecycleName });

            var channelName = $"Channel-{Guid.NewGuid()}";
            var channel = new ChannelResource()
            {
                Id = Guid.NewGuid().ToString(),
                Name = channelName
            };

            Repository.Projects.GetChannels(Arg.Any<ProjectResource>())
                .Returns(new ResourceCollection<ChannelResource>(new[] { channel }, new LinkCollection()));

            CommandLineArgs.Add($"--channel={channelName}");
            CommandLineArgs.Add($"--project={projectName}");
            CommandLineArgs.Add($"--lifecycle={lifecycleName}");
            CommandLineArgs.Add("--update-existing");

            createChannelCommand.Execute(CommandLineArgs.ToArray());

            Log.Received().Information("Channel {0} updated", channelName);
        }
    }
}