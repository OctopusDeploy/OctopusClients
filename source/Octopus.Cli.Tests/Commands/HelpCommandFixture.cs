using System;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Tests.Helpers;
using FluentAssertions;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class HelpCommandFixture
    {
        HelpCommand helpCommand;
        ICommandLocator commandLocator;
        StringWriter output;
        TextWriter originalOutput;

        [SetUp]
        public void SetUp()
        {
            originalOutput = Console.Out;
            Console.SetOut(output = new StringWriter());

            commandLocator = Substitute.For<ICommandLocator>();
            helpCommand = new HelpCommand(commandLocator);
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
                .Contain("Usage: Octo <command> [<options>]")
                .And.Contain("Where <command> is one of:")
                .And.Contain("create-foo");
        }

        [Test]
        public void ShouldPrintHelpForExistingCommand()
        {
            var speak = Substitute.For<ICommand>();
            commandLocator.Find("speak").Returns(speak);
            helpCommand.Execute("speak");

            output.ToString()
                .Should()
                .Contain("Usage: Octo speak [<options>]");

            speak.Received().GetHelp(Arg.Any<TextWriter>());
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
