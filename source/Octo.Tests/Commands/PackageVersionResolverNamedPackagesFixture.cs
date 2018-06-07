using NUnit.Framework;
using Octopus.Cli.Commands.Releases;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Tests.Util;
using Serilog;

namespace Octo.Tests.Commands
{
    [TestFixture]
    public class PackageVersionResolverNamedPackagesFixture
    {
        PackageVersionResolver resolver;
        FakeOctopusFileSystem fileSystem;

        [SetUp]
        public void SetUp()
        {
            var log = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Trace().CreateLogger();
            fileSystem = new FakeOctopusFileSystem();
            resolver = new PackageVersionResolver(log, fileSystem);
        }

        [Test]
        public void ShouldReturnPackageVersionToUse()
        {
            resolver.Add("PackageA", "Package1", "1.0.0");
            resolver.Add("PackageB", "Package1", "1.1.0");

            Assert.That(resolver.ResolveVersion("Step", "Package1", "PackageA"), Is.EqualTo("1.0.0"));
            Assert.That(resolver.ResolveVersion("Step", "Package1", "PackageB"), Is.EqualTo("1.1.0"));
        }

        [Test]
        public void ShouldBeCaseInsensitive()
        {
            resolver.Add("PackageA", "Package1", "1.0.0");
            resolver.Add("packageA", "Package1", "1.1.0");

            Assert.That(resolver.ResolveVersion("Step", "Package1", "PackageA"), Is.EqualTo("1.1.0"));
            Assert.That(resolver.ResolveVersion("Step", "Package1", "packagea"), Is.EqualTo("1.1.0"));
        }

        [Test]
        public void ShouldReturnHighestWhenConflicts()
        {
            resolver.Add("PackageA", "Package1", "1.0.0");
            resolver.Add("PackageA", "Package1", "1.1.0");
            resolver.Add("PackageA", "Package1", "0.9.0");

            Assert.That(resolver.ResolveVersion("Step", "Package1", "PackageA"), Is.EqualTo("1.1.0"));
        }

        [Test]
        public void ShouldReturnNullForUnknownSelection()
        {
            resolver.Add("PackageA", "Package1", "1.0.0");

            Assert.That(resolver.ResolveVersion("Step", "Package1", "PackageA"), Is.EqualTo("1.0.0"));
            Assert.That(resolver.ResolveVersion("Step", "Package1", "PackageZ"), Is.Null);
        }

        [Test]
        public void ShouldReturnDefaultWhenSet()
        {
            resolver.Default("2.91.0");

            Assert.That(resolver.ResolveVersion("Step", "Package1", "PackageA"), Is.EqualTo("2.91.0"));
            Assert.That(resolver.ResolveVersion("Step", "Package1", "PackageB"), Is.EqualTo("2.91.0"));
            Assert.That(resolver.ResolveVersion("Step", "Package1", "PackageC"), Is.EqualTo("2.91.0"));
        }

        [Test]
        public void ShouldParseConstraint()
        {
            resolver.Add("PackageA:Package1:1.0.0");
            resolver.Add("PackageB:Package1:1.0.0-alpha1");
            resolver.Add("PackageB=Package1=1.0.0-alpha1");

            Assert.That(resolver.ResolveVersion("Step", "Package1", "PackageA"), Is.EqualTo("1.0.0"));
            Assert.That(resolver.ResolveVersion("Step", "Package1", "PackageB"), Is.EqualTo("1.0.0-alpha1"));
        }

        [Test]
        public void ShouldThrowOnInvalidConstraint()
        {
            Assert.Throws<CommandException>(() => resolver.Add(":"));
            Assert.Throws<CommandException>(() => resolver.Add("="));
            Assert.Throws<CommandException>(() => resolver.Add(":Package1:1.0.0"));
            Assert.Throws<CommandException>(() => resolver.Add("=Package1=1.0.0"));
            Assert.Throws<CommandException>(() => resolver.Add("PackageA:Package1"));
            Assert.Throws<CommandException>(() => resolver.Add("PackageA:Package1:1.FRED.9"));
            Assert.Throws<CommandException>(() => resolver.Add("PackageA=Package1=1.FRED.9"));
        }

        [Test]
        public void ShouldPreferStepNameToPackageId()
        {
            resolver.Default("1.0.0");
            resolver.Add("StepName", "Package1", "1.1.0");
            resolver.Add("PackageId", "Package1", "1.2.0");
            Assert.That(resolver.ResolveVersion("StepName", "Package1", "PackageId"), Is.EqualTo("1.1.0"));
        }


        [Test]
        public void ShouldPreferPackageIdToDefault()
        {
            resolver.Default("1.0.0");
            resolver.Add("OtherStep", "Package1", "1.1.0");
            resolver.Add("PackageId", "Package1", "1.2.0");

            Assert.That(resolver.ResolveVersion("StepName", "Package1", "PackageId"), Is.EqualTo("1.2.0"));
        }
    }
}