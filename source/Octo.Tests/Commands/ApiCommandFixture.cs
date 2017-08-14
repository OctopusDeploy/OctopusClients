using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Octo.Commands;
using Octopus.Cli.Commands;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Tests.Helpers;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class ApiCommandFixture : ApiCommandFixtureBase
    {
        DummyApiCommand apiCommand;

        [SetUp]
        public void SetUp()
        {
            apiCommand = new DummyApiCommand(RepositoryFactory, Log, FileSystem, ClientFactory, CommandOutputProvider);
        }

        [Test]
        public void ShouldThrowIfNoServerSpecified()
        {
            Assert.Throws<CommandException>(() => apiCommand.Execute("--apiKey=ABCDEF123456789"));
        }

        [Test]
        public void ShouldThrowIfNoApiKeySpecified()
        {
            Assert.Throws<CommandException>(() => apiCommand.Execute("--server=http://the-server"));
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
    }
}
