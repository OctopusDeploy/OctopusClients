using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OctopusTools.Infrastructure;

namespace OctopusTools.Tests.Commands
{
    [TestFixture]
    public class ApiCommandFixture : ApiCommandFixtureBase
    {
        DummyApiCommand apiCommand;

        [SetUp]
        public void SetUp()
        {
            apiCommand = new DummyApiCommand(RepositoryFactory, Log, FileSystem);
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
            Assert.Throws<CommandException>(() => apiCommand.Execute(CommandLineArgs.ToArray()));
        }

        [Test]
        public void ShouldNotThrowIfCustomOptionsAreAddedByCommand()
        {
            CommandLineArgs.Add("--pill=red");
            apiCommand.Execute(CommandLineArgs.ToArray());
        }

        [Test]
        public void ShouldExecuteCommandWhenCorrectCommandLineParametersArePassed()
        {
            apiCommand.Execute(CommandLineArgs.ToArray());
        }

    }
}
