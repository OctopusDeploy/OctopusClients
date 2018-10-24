using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Repositories;
using Octopus.Cli.Tests.Helpers;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octo.Tests.Commands
{
    [TestFixture]
    public class ApiCommandFixture : ApiCommandFixtureBase
    {
        DummyApiCommand apiCommand;

        [SetUp]
        public void SetUp()
        {
            apiCommand = new DummyApiCommand(RepositoryFactory, FileSystem, ClientFactory, CommandOutputProvider);
        }

        [Test]
        public void ShouldThrowIfNoServerSpecified()
        {
            Environment.SetEnvironmentVariable(ApiCommand.ServerUrlEnvVar, "");
            Assert.Throws<CommandException>(() => apiCommand.Execute("--apiKey=ABCDEF123456789"));    
        }

        [Test]
        public void ShouldThrowIfNoApiKeySpecified()
        {
            Environment.SetEnvironmentVariable(ApiCommand.ApiKeyEnvVar, "");
            Assert.Throws<CommandException>(() => apiCommand.Execute("--server=http://the-server"));
        }
        
        [Test]
        public void ShouldNotThrowIfApiKeySetInEnvVar()
        {
            Environment.SetEnvironmentVariable(ApiCommand.ApiKeyEnvVar, "whatever");
            apiCommand.Execute("--server=http://the-server");
        }
        
        [Test]
        public void ShouldNotThrowIfServerSetInEnvVar()
        {
            Environment.SetEnvironmentVariable(ApiCommand.ServerUrlEnvVar, "http://whatever");
            apiCommand.Execute("--apiKey=ABCDEF123456789");
        }

        [Test]
        public void ShouldThrowIfInvalidCommandLineParametersArePassed()
        {
            CommandLineArgs.Add("--fail=epic");
            Func<Task> exec = () => apiCommand.Execute(CommandLineArgs.ToArray());
            exec.ShouldThrow<CommandException>();
        }

        [Test]
        public Task ShouldNotThrowIfCustomOptionsAreAddedByCommand()
        {
            CommandLineArgs.Add("--pill=red");
            return apiCommand.Execute(CommandLineArgs.ToArray());
        }

        [Test]
        public Task ShouldExecuteCommandWhenCorrectCommandLineParametersArePassed()
        {
            return apiCommand.Execute(CommandLineArgs.ToArray());
        }

        [Test]
        public Task ShouldExecuteCommandWhenCorrectCommandLineParametersArePassedWithSpaceName()
        {
            var clientFactory = Substitute.For<IOctopusClientFactory>();
            var client = Substitute.For<IOctopusAsyncClient>();

            var repositoryFactory = Substitute.For<IOctopusAsyncRepositoryFactory>();
            
            clientFactory.CreateAsyncClient(Arg.Any<OctopusServerEndpoint>(), Arg.Any<OctopusClientOptions>())
                .Returns(client);
            var systemRepository = Substitute.For<IOctopusSystemAsyncRepository>();
            systemRepository.Spaces.FindByName(Arg.Any<string>()).Returns(new SpaceResource {Id = "Spaces-2"});
            client.ForSystem().Returns(systemRepository);

            apiCommand = new DummyApiCommand(repositoryFactory, FileSystem, clientFactory, CommandOutputProvider);
            var argsWithSpaceName = CommandLineArgs.Concat(new []{"--spaceName=abc"});
            return apiCommand.Execute(argsWithSpaceName.ToArray());
        }
    }
}
