using System;
using NUnit.Framework;
using Octopus.Cli.Tests.Helpers;
using Octopus.Client;

namespace Octopus.Cli.Tests
{
    [TestFixture]
    public class VersioningFixture
    {
        [Test]
        public void TheTestAssemblyIsVersioned_WithGitVersion()
        {
            Assert.That(new SemanticVersionInfo(GetType().Assembly).Major, Is.GreaterThan(1));
        }

        [Test]
        public void AllOctopusAssemblies_ShouldHaveTheSameSemanticVersion()
        {
            var octocliVersion = (new SemanticVersionInfo(typeof(Octopus.Cli.Program).Assembly));
            var octopusClientVersion = new SemanticVersionInfo(typeof(IOctopusRepository).Assembly);

            var isClientPreRelease = !string.IsNullOrEmpty(octopusClientVersion.PreReleaseTag);
            var isThisPreRelease = !string.IsNullOrEmpty(octocliVersion.PreReleaseTag);

            Console.WriteLine($"Octopus.Client: {octopusClientVersion.SemVer}");
            Console.WriteLine($"Octopus.Cli (Octo.exe): {octocliVersion.SemVer}");

            if (isClientPreRelease) Assert.That(isThisPreRelease, "We are using a pre-release version of Octopus.Client, so octo.exe should also be versioned as a pre-release. We should only build full-releases of octo.exe using full releases of Octopus.Client.");
            else Assert.That(true);
        }
    }
}
