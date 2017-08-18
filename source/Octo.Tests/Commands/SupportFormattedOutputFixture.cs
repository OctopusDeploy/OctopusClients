using System.IO;
using FluentAssertions;
using NUnit.Framework;
using Octopus.Cli.Tests.Helpers;
using Serilog;
using System.Threading.Tasks;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class SupportFormattedOutputFixture : ApiCommandFixtureBase
    {
        [Test]
        public void FormattedOutput_ShouldAddOutputOption()
        {
            // arrange
            StringWriter sw = new StringWriter();
            DummyApiCommandWithFormattedOutputSupport command =
                new DummyApiCommandWithFormattedOutputSupport(ClientFactory, RepositoryFactory, Log, FileSystem,
                    CommandOutputProvider);

            // act
            command.GetHelp(sw, new []{ "command" });

            // assert
            sw.ToString().Should().ContainEquivalentOf("--output");
        }

        [Test]
        public async Task FormattedOutput_FormatSetToJson()
        {
            // arrange
            DummyApiCommandWithFormattedOutputSupport command =
                new DummyApiCommandWithFormattedOutputSupport(ClientFactory, RepositoryFactory, Log, FileSystem, CommandOutputProvider);

            CommandLineArgs.Add("--output=json");

            // act
            await command.Execute(CommandLineArgs.ToArray());

            // assert
            command.PrintJsonOutputCalled.ShouldBeEquivalentTo(true);
        }

        [Test]
        public async Task FormattedOutput_FormatSetToXml()
        {
            // arrang
            DummyApiCommandWithFormattedOutputSupport command =
                new DummyApiCommandWithFormattedOutputSupport(ClientFactory, RepositoryFactory, Log, FileSystem,
                    CommandOutputProvider);
            CommandLineArgs.Add("--output=xml");

            // act
            await command.Execute(CommandLineArgs.ToArray());
            
            // assert
            command.PrintXmlOutputCalled.ShouldBeEquivalentTo(true);
        }

        [Test]
        public async Task FormattedOutput_FormatInvalid()
        {
            // arrang
            DummyApiCommandWithFormattedOutputSupport command =
                new DummyApiCommandWithFormattedOutputSupport(ClientFactory, RepositoryFactory, Log, FileSystem,
                    CommandOutputProvider);
            CommandLineArgs.Add("--output=blah");

            // act
            await command.Execute(CommandLineArgs.ToArray());

            // assert
            command.PrintXmlOutputCalled.ShouldBeEquivalentTo(false);
            command.PrintJsonOutputCalled.ShouldBeEquivalentTo(false);
            command.PrintDefaultOutputCalled.ShouldBeEquivalentTo(true);
        }

    }
}