using System;
using System.IO;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Cli.Infrastructure;
using Serilog;

namespace Octopus.Cli.Tests.Commands
{
    [TestFixture]
    public class PackageVersionResolverFixture
    {
        PackageVersionResolver resolver;

        [SetUp]
        public void SetUp()
        {
            Program.ConfigureLogger();
            resolver = new PackageVersionResolver(Log.Logger);
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


        [Test]
        public void ShouldDetermineVersionFromZipFile()
        {
            // create test files, content does not matter as only file name is used
            var path = Path.Combine(Path.GetTempPath(), "octo-test-" + Guid.NewGuid());
            Directory.CreateDirectory(path);
            File.WriteAllText(Path.Combine(path, "Package.1.2.3.zip"), string.Empty);
            File.WriteAllText(Path.Combine(path, "Package2.1.2.3-alpha-1.zip"), string.Empty);
            File.WriteAllText(Path.Combine(path, "My.Package.2017.2.3.4-alpha-quality.zip"), string.Empty);
            File.WriteAllText(Path.Combine(path, "Family_photos.zip"), string.Empty);

            resolver.AddFolder(path);

            Assert.That(resolver.ResolveVersion("Package"), Is.EqualTo("1.2.3"));
            Assert.That(resolver.ResolveVersion("Package2"), Is.EqualTo("1.2.3-alpha-1"));
            Assert.That(resolver.ResolveVersion("My.Package"), Is.EqualTo("2017.2.3.4-alpha-quality"));
            Assert.That(resolver.ResolveVersion("Family_photos"), Is.Null);
        }
    }
}