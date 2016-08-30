using System;
using NuGet.Versioning;
using NUnit.Framework;
using Octopus.Cli.Tests.Helpers;
using Octopus.Cli.Util;
using Octopus.Client;

namespace Octopus.Cli.Tests
{
    [TestFixture]
    public class VersioningFixture
    {
        [Test]
        public void TheOctoAssemblyIsVersioned_WithGitVersion()
        {
            Assert.That(SemanticVersion.Parse(typeof(Program).GetInformationalVersion()).Major, Is.GreaterThan(1));
        }

        [Test]
        public void AllOctopusAssemblies_ShouldHaveTheSameSemanticVersion()
        {
            var octocliVersion = SemanticVersion.Parse(typeof(Program).GetInformationalVersion());
            var octopusClientVersion = SemanticVersion.Parse(typeof(IOctopusClient).GetInformationalVersion());


            Console.WriteLine($"Octopus.Client: {octopusClientVersion}");
            Console.WriteLine($"Octopus.Cli (Octo.exe): {octocliVersion}");

            if (octopusClientVersion.IsPrerelease)
                Assert.That(octocliVersion.IsPrerelease, "We are using a pre-release version of Octopus.Client, so octo.exe should also be versioned as a pre-release. We should only build full-releases of octo.exe using full releases of Octopus.Client.");
        }
    }
}
