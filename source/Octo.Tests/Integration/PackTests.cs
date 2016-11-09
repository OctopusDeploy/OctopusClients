using System;
using System.Linq;
using System.IO;
using FluentAssertions;
using NUnit.Framework;

namespace Octopus.Cli.Tests.Integration
{
    public class PackTests
    {
        [Test]
        public void PackZip()
        {
            Execute("zip");
        }

        [Test]
        public void PackNuget()
        {
            Execute("nupkg");
        }

        private static void Execute(string format)
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            try
            {
                var directory = Directory.CreateDirectory(tempFolder);
                File.WriteAllText(Path.Combine(tempFolder, "Test.txt"), "Test");

                var result = Program.Run(
                    new[]
                    {
                        "pack",
                        "--id=TestPackage",
                        $"--basePath={tempFolder}",
                        $"--outFolder={tempFolder}",
                        "--version=1.2.3-beta",
                        $"--format={format}"
                    }
                );

                result.Should().Be(0);
                directory.GetFiles()
                    .Select(f => f.Name)
                    .Should()
                    .Contain($"TestPackage.1.2.3-beta.{format}");
            }
            finally
            {
                Directory.Delete(tempFolder, true);
            }
        }
    }
}