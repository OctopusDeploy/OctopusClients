using System;
using System.Reflection;
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
            Assert.That(new SemanticVersionInfo(GetType().GetTypeInfo().Assembly).Major, Is.GreaterThan(1));
        }

        [Test]
        public void AllOctopusAssemblies_ShouldHaveTheSameSemanticVersion()
        {
            var octocliVersion = (new SemanticVersionInfo(typeof(Octopus.Cli.Program).GetTypeInfo().Assembly));
            var octopusClientVersion = new SemanticVersionInfo(typeof(IOctopusRepository).GetTypeInfo().Assembly);

            var isClientPreRelease = !string.IsNullOrEmpty(octopusClientVersion.PreReleaseTag);
            var isThisPreRelease = !string.IsNullOrEmpty(octocliVersion.PreReleaseTag);

            Console.WriteLine($"Octopus.Client: {octopusClientVersion.SemVer}");
            Console.WriteLine($"Octopus.Cli (Octo.exe): {octocliVersion.SemVer}");

            Assert.That(!(isClientPreRelease ^ isThisPreRelease), "Octo.exe must be a pre-release if Octopus.Client is a pre-release, and non pre-release if Octopus.Client is not a pre-release");
        }
    }
}
