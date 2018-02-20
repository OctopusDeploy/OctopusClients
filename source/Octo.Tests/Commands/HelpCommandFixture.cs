using System;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Tests.Helpers;
using FluentAssertions;
using Octopus.Cli.Commands;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Util;
using Serilog;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class HelpCommandFixture
    {
        HelpCommand helpCommand;
        ICommandLocator commandLocator;
        StringWriter output;
        TextWriter originalOutput;
        private ICommandOutputProvider commandOutputProvider;
        private ILogger logger;

        [SetUp]
        public void SetUp()
        {
            originalOutput = Console.Out;
            output = new StringWriter();
            Console.SetOut(output);

            commandLocator = Substitute.For<ICommandLocator>();
            commandOutputProvider = new CommandOutputProvider(logger);
            helpCommand = new HelpCommand(commandLocator, commandOutputProvider);
            logger = new LoggerConfiguration().WriteTo.TextWriter(output).CreateLogger();
        }

        [Test]
        public void ShouldPrintGeneralHelpWhenNoArgsGiven()
        {
            commandLocator.List().Returns(new ICommandMetadata[]
            {
                new Metadata() { Name = "create-foo"},
                new Metadata() { Name = "create-bar"}
            });

            helpCommand.Execute();

            output.ToString()
                .Should()
                .Contain("Usage: octo <command> [<options>]")
                .And.Contain("Where <command> is one of:")
                .And.Contain("create-foo");
        }

        [Test]
        public void ShouldPrintHelpForExistingCommand()
        {
            var speak = new SpeakCommand(commandOutputProvider);
            commandLocator.Find("speak").Returns(speak);
            helpCommand.Execute("speak");

            output.ToString()
                .Should()
                .Contain("Usage: Octo.Tests speak [<options>]");
        }

        [Test]
        public void ShouldFailForUnrecognisedCommand()
        {
            commandLocator.Find("foo").Returns((ICommand)null);
            helpCommand.Execute("foo");

            Assert.That(output.ToString(), Does.Contain("Command 'foo' is not supported"));
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetOut(originalOutput);
        }

        private class Metadata : ICommandMetadata
        {
            public string Name { get; set; }
            public string[] Aliases { get; set; }
            public string Description { get; set; }
        }
    }
}
