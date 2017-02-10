using System;
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

            Assert.That(resolver.ResolveVersion("PackageA"), Is.EqualTo("1.0.0"));
            Assert.That(resolver.ResolveVersion("PackageB"), Is.EqualTo("1.1.0"));
        }

        [Test]
        public void ShouldBeCaseInsensitive()
        {
            resolver.Add("PackageA", "1.0.0");
            resolver.Add("packageA", "1.1.0");

            Assert.That(resolver.ResolveVersion("PackageA"), Is.EqualTo("1.1.0"));
            Assert.That(resolver.ResolveVersion("packagea"), Is.EqualTo("1.1.0"));
        }

        [Test]
        public void ShouldReturnHighestWhenConflicts()
        {
            resolver.Add("PackageA", "1.0.0");
            resolver.Add("PackageA", "1.1.0");
            resolver.Add("PackageA", "0.9.0");

            Assert.That(resolver.ResolveVersion("PackageA"), Is.EqualTo("1.1.0"));
        }

        [Test]
        public void ShouldReturnNullForUnknownSelection()
        {
            resolver.Add("PackageA", "1.0.0");

            Assert.That(resolver.ResolveVersion("PackageA"), Is.EqualTo("1.0.0"));
            Assert.That(resolver.ResolveVersion("PackageZ"), Is.Null);
        }

        [Test]
        public void ShouldReturnDefaultWhenSet()
        {
            resolver.Default("2.91.0");

            Assert.That(resolver.ResolveVersion("PackageA"), Is.EqualTo("2.91.0"));
            Assert.That(resolver.ResolveVersion("PackageB"), Is.EqualTo("2.91.0"));
            Assert.That(resolver.ResolveVersion("PackageC"), Is.EqualTo("2.91.0"));
        }

        [Test]
        public void ShouldParseConstraint()
        {
            resolver.Add("PackageA:1.0.0");
            resolver.Add("PackageB:1.0.0-alpha1");
            resolver.Add("PackageB=1.0.0-alpha1");

            Assert.That(resolver.ResolveVersion("PackageA"), Is.EqualTo("1.0.0"));
            Assert.That(resolver.ResolveVersion("PackageB"), Is.EqualTo("1.0.0-alpha1"));
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


        [TestCase("acme.1.2.0", "acme", "1.2.0", true)]
        [TestCase("acme-1.2.0", "acme", "1.2.0", true)]
        [TestCase("acme.1.2.0.10", "acme", "1.2.0.10", true)]
        [TestCase("acme-1.2.0.10", "acme", "1.2.0.10", true)]
        [TestCase("acme.1", "acme", "1", true)]
        [TestCase("acme-1", "acme", "1", true)]
        [TestCase("acme.1.2", "acme", "1.2", true)]
        [TestCase("acme.web.1.2.56", "acme.web", "1.2.56", true)]
        [TestCase("acme.web.1.2.0-alpha", "acme.web", "1.2.0-alpha", true)]
        [TestCase("acme.web.1.2.0-alpha.1.22", "acme.web", "1.2.0-alpha.1.22", true)]
        [TestCase("acme.web-1.2.0-alpha.1.22", "acme.web", "1.2.0-alpha.1.22", true)]
        [TestCase("acme.web.1.2.0+build", "acme.web", "1.2.0+build", true)]
        [TestCase("acme.web-1.2.0+build", "acme.web", "1.2.0+build", true)]
        [TestCase("acme.web.1.2.0-alpha.1+build", "acme.web", "1.2.0-alpha.1+build", true)]
        [TestCase("acme.web-1.2.0-alpha.1+build", "acme.web", "1.2.0-alpha.1+build", true)]
        [TestCase("acme+web.1", "", "", false)]
        [TestCase("acme+web-1", "", "", false)]
        [TestCase("acme.web.1.0.0.0.0", "", "", false)]
        [TestCase("acme.web-1.0.0.0.0", "", "", false)]
        public void CanParseIdAndVersion(string input, string expectedPackageId, string expectedVersion, bool canParse)
        {
            var filename = Path.Combine("temp", $"{input}.zip");
            fileSystem.Files[filename] = "";

            resolver.AddFolder(Path.GetDirectoryName(filename));

            var result = resolver.ResolveVersion(expectedPackageId);
            if (canParse)
                result.Should().Be(expectedVersion);
            else
                result.Should().BeNull();
        }
    }
}