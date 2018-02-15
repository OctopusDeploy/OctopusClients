using System;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Cli.Tests.Helpers;
using Octopus.Cli.Util;
using Serilog;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class VersionTestFixture
    {
        VersionCommand versionCommand;
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

            commandOutputProvider = new CommandOutputProvider(logger);
            versionCommand = new VersionCommand(commandOutputProvider);
            logger = new LoggerConfiguration().WriteTo.TextWriter(output).CreateLogger();
        }

        [Test]
        public void ShouldPrintCorrectVersionNumber()
        {
            var version = GetVersionFromFile(Path.Combine(TestContext.CurrentContext.WorkDirectory, "ExpectedSdkVersion.txt"));

            versionCommand.Execute();

            output.ToString()
                .Should()
                .Contain(version);
        }

        private string GetVersionFromFile(string versionFilePath)
        {
            using (var reader = new StreamReader(File.OpenRead(versionFilePath)))
            {
                return reader.ReadLine();
            }
        }

    }
}
