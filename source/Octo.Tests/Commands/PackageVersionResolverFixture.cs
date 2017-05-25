using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Tests.Util;
using Octopus.Cli.Util;
using Serilog;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class PackageVersionResolverFixture
    {
        PackageVersionResolver resolver;
        FakeOctopusFileSystem fileSystem;

        [SetUp]
        public void SetUp()
        {
            Program.ConfigureLogger();
            fileSystem = new FakeOctopusFileSystem();
            resolver = new PackageVersionResolver(Log.Logger, fileSystem);
        }

        [Test]
        public void ShouldReturnPackageVersionToUse()
        {
            resolver.Add("PackageA", "1.0.0");
            resolver.Add("PackageB", "1.1.0");

            Assert.That(resolver.ResolveVersion("Step", "PackageA"), Is.EqualTo("1.0.0"));
            Assert.That(resolver.ResolveVersion("Step", "PackageB"), Is.EqualTo("1.1.0"));
        }

        [Test]
        public void ShouldBeCaseInsensitive()
        {
            resolver.Add("PackageA", "1.0.0");
            resolver.Add("packageA", "1.1.0");

            Assert.That(resolver.ResolveVersion("Step", "PackageA"), Is.EqualTo("1.1.0"));
            Assert.That(resolver.ResolveVersion("Step", "packagea"), Is.EqualTo("1.1.0"));
        }

        [Test]
        public void ShouldReturnHighestWhenConflicts()
        {
            resolver.Add("PackageA", "1.0.0");
            resolver.Add("PackageA", "1.1.0");
            resolver.Add("PackageA", "0.9.0");

            Assert.That(resolver.ResolveVersion("Step", "PackageA"), Is.EqualTo("1.1.0"));
        }

        [Test]
        public void ShouldReturnNullForUnknownSelection()
        {
            resolver.Add("PackageA", "1.0.0");

            Assert.That(resolver.ResolveVersion("Step", "PackageA"), Is.EqualTo("1.0.0"));
            Assert.That(resolver.ResolveVersion("Step", "PackageZ"), Is.Null);
        }

        [Test]
        public void ShouldReturnDefaultWhenSet()
        {
            resolver.Default("2.91.0");

            Assert.That(resolver.ResolveVersion("Step", "PackageA"), Is.EqualTo("2.91.0"));
            Assert.That(resolver.ResolveVersion("Step", "PackageB"), Is.EqualTo("2.91.0"));
            Assert.That(resolver.ResolveVersion("Step", "PackageC"), Is.EqualTo("2.91.0"));
        }

        [Test]
        public void ShouldParseConstraint()
        {
            resolver.Add("PackageA:1.0.0");
            resolver.Add("PackageB:1.0.0-alpha1");
            resolver.Add("PackageB=1.0.0-alpha1");

            Assert.That(resolver.ResolveVersion("Step", "PackageA"), Is.EqualTo("1.0.0"));
            Assert.That(resolver.ResolveVersion("Step", "PackageB"), Is.EqualTo("1.0.0-alpha1"));
        }

        [Test]
        public void ShouldThrowOnInvalidConstraint()
        {
            Assert.Throws<CommandException>(() => resolver.Add(":"));
            Assert.Throws<CommandException>(() => resolver.Add("="));
            Assert.Throws<CommandException>(() => resolver.Add(":1.0.0"));
            Assert.Throws<CommandException>(() => resolver.Add("=1.0.0"));
            Assert.Throws<CommandException>(() => resolver.Add("PackageA:"));
            Assert.Throws<CommandException>(() => resolver.Add("PackageA:1.FRED.9"));
            Assert.Throws<CommandException>(() => resolver.Add("PackageA=1.FRED.9"));
        }

        [Test]
        public void ShouldPreferStepNameToPackageId()
        {
            resolver.Default("1.0.0");
            resolver.Add("StepName", "1.1.0");
            resolver.Add("PackageId", "1.2.0");
            Assert.That(resolver.ResolveVersion("StepName", "PackageId"), Is.EqualTo("1.1.0"));
        }


        [Test]
        public void ShouldPreferPackageIdToDefault()
        {
            resolver.Default("1.0.0");
            resolver.Add("OtherStep", "1.1.0");
            resolver.Add("PackageId", "1.2.0");

            Assert.That(resolver.ResolveVersion("StepName", "PackageId"), Is.EqualTo("1.2.0"));
        }


        public static IEnumerable<TestCaseData> CanParseIdAndVersionData()
        {
            var extensions = new[] { ".zip", ".tgz", ".tar.gz", ".tar.Z", ".tar.bz2", ".tar.bz", ".tbz", ".tar" };
            foreach (var ext in extensions)
            {
                yield return CreateCanParseIdAndVersionCase("acme", "1.2.0", ext);
                yield return CreateCanParseIdAndVersionCase("acme", "1.2.0", ext);
                yield return CreateCanParseIdAndVersionCase("acme", "1.2.0.10", ext);
                yield return CreateCanParseIdAndVersionCase("acme", "1.2.0.10", ext);
                yield return CreateCanParseIdAndVersionCase("acme", "1", ext);
                yield return CreateCanParseIdAndVersionCase("acme", "1", ext);
                yield return CreateCanParseIdAndVersionCase("acme", "1.2", ext);
                yield return CreateCanParseIdAndVersionCase("acme.web", "1.2.56", ext);
                yield return CreateCanParseIdAndVersionCase("acme.web", "1.2.0-alpha", ext);
                yield return CreateCanParseIdAndVersionCase("acme.web", "1.2.0-alpha.1.22", ext);
                yield return CreateCanParseIdAndVersionCase("acme.web", "1.2.0-alpha.1.22", ext);
                yield return CreateCanParseIdAndVersionCase("acme.web", "1.2.0+build", ext);
                yield return CreateCanParseIdAndVersionCase("acme.web", "1.2.0+build", ext);
                yield return CreateCanParseIdAndVersionCase("acme.web", "1.2.0-alpha.1+build", ext);
                yield return CreateCanParseIdAndVersionCase("acme.web", "1.2.0-alpha.1+build", ext);
            }

            var invalid = new[]
            {
                "acme+web.1.zip",
                "acme.web.1.0.0.0.0.zip",
                "acme.web-1.0.0.zip"
            };
            yield return new TestCaseData("acme+web.1.zip", false, "acme+web", null).SetName("acme+web.1.zip");
            yield return new TestCaseData("acme.web.1.0.0.0.0.zip", false, "acme.web", null).SetName("acme.web.1.0.0.0.0.zip");
            yield return new TestCaseData("acme.web-1.0.0.zip", false, "acme.web", null).SetName("acme.web-1.0.0.zip");
        }

        private static TestCaseData CreateCanParseIdAndVersionCase(string packageId, string version, string ext)
        {
            var filename = $"{packageId}.{version}{ext}";
            return new TestCaseData(filename, true, packageId, version)
                .SetName(filename);
        }

        [TestCaseSource(nameof(CanParseIdAndVersionData))]
        public void CanParseIdAndVersion(string filename, bool canParse, string expectedPackageId, string expectedVersion)
        {
            var path = Path.Combine("temp", filename);
            fileSystem.Files[path] = "";

            resolver.AddFolder(Path.GetDirectoryName(filename));

            var result = resolver.ResolveVersion("SomeStep", expectedPackageId);
            if (canParse)
                result.Should().Be(expectedVersion);
            else
                result.Should().BeNull();
        }
    }
}