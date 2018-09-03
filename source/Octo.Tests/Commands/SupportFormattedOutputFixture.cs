using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Octo.Tests.Commands
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
                new DummyApiCommandWithFormattedOutputSupport(ClientFactory, RepositoryFactory, FileSystem, CommandOutputProvider);

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
                new DummyApiCommandWithFormattedOutputSupport(ClientFactory, RepositoryFactory, FileSystem, CommandOutputProvider);

            CommandLineArgs.Add("--outputFormat=json");

            // act
            await command.Execute(CommandLineArgs.ToArray());

            // assert
            command.PrintJsonOutputCalled.ShouldBeEquivalentTo(true);
        }
        
        [Test]
        public async Task FormattedOutput_FormatInvalid()
        {
            // arrange
            DummyApiCommandWithFormattedOutputSupport command =
                new DummyApiCommandWithFormattedOutputSupport(ClientFactory, RepositoryFactory, FileSystem, CommandOutputProvider);
            CommandLineArgs.Add("--helpOutputFormat=blah");

            // act
            await command.Execute(CommandLineArgs.ToArray());

            // assert
            command.PrintJsonOutputCalled.ShouldBeEquivalentTo(false);
            command.PrintDefaultOutputCalled.ShouldBeEquivalentTo(true);
        }

        [Test]
        public async Task FormattedOutputHelp_ShouldBeWellFormed()
        {
            // arrange
            DummyApiCommandWithFormattedOutputSupport command =
                new DummyApiCommandWithFormattedOutputSupport(ClientFactory, RepositoryFactory, FileSystem, CommandOutputProvider);

            CommandLineArgs.Add("--helpOutputFormat=json");
            CommandLineArgs.Add("--help");

            // act
            await command.Execute(CommandLineArgs.ToArray());

            // assert
            var logoutput = LogOutput.ToString();
            Console.WriteLine(logoutput);
            JsonConvert.DeserializeObject(logoutput);
            logoutput.Should().Contain("--helpOutputFormat=VALUE");
            logoutput.Should().Contain("--help");

        }

    }
}