using System;
using System.IO;
using NSubstitute;
using NUnit.Framework;
using OctopusTools.Commands;
using OctopusTools.Infrastructure;

namespace OctopusTools.Tests.Commands
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
            
            Assert.That(output.ToString(), 
                Is.StringContaining("Usage: Octo <command> [<options>]").And
                .StringContaining("Where <command> is one of:").And
                .StringContaining("create-foo"));
        }

        [Test]
        public void ShouldPrintHelpForExistingCommand()
        {
            commandLocator.Find("speak").Returns(new SpeakCommand());
            helpCommand.Execute("speak");

            Assert.That(output.ToString(),
                Is.StringContaining("Usage: Octo speak [<options>]").And
                .StringContaining("message="));
        }

        [Test]
        public void ShouldFailForUnrecognisedCommand()
        {
            commandLocator.Find("foo").Returns((ICommand)null);
            helpCommand.Execute("foo");

            Assert.That(output.ToString(), Is.StringContaining("Command 'foo' is not supported"));
        }

        [TearDown]
        public void TearDown()
        {
            Console.SetOut(originalOutput);
        }

        private class SpeakCommand : ICommand
        {
            public OptionSet Options
            {
                get 
                { 
                    var options = new OptionSet();
                    options.Add("message=", m => { });
                    return options;
                }
            }

            public void Execute()
            {
                Assert.Fail("This should not be called");
            }
        }

        private class Metadata : ICommandMetadata
        {
            public string Name { get; set; }
            public string[] Aliases { get; set; }
            public string Description { get; set; }
        }
    }
}
