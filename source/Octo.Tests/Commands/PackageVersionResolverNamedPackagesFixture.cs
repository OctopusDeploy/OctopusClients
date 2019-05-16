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

            Assert.That(resolver.ResolveVersion("Step", "PackageA", "Package1"), Is.EqualTo("1.0.0"));
            Assert.That(resolver.ResolveVersion("Step", "PackageB", "Package1"), Is.EqualTo("1.1.0"));
        }

        [Test]
        public void ShouldBeCaseInsensitive()
        {
            resolver.Add("PackageA", "Package1", "1.0.0");
            resolver.Add("packageA", "Package1", "1.1.0");

            Assert.That(resolver.ResolveVersion("Step", "PackageA", "Package1"), Is.EqualTo("1.1.0"));
            Assert.That(resolver.ResolveVersion("Step", "packagea", "Package1"), Is.EqualTo("1.1.0"));
        }

        [Test]
        public void ShouldReturnHighestWhenConflicts()
        {
            resolver.Add("PackageA", "Package1", "1.0.0");
            resolver.Add("PackageA", "Package1", "1.1.0");
            resolver.Add("PackageA", "Package1", "0.9.0");

            Assert.That(resolver.ResolveVersion("Step", "PackageA", "Package1"), Is.EqualTo("1.1.0"));
        }

        [Test]
        public void ShouldReturnNullForUnknownSelection()
        {
            resolver.Add("PackageA", "Package1", "1.0.0");

            Assert.That(resolver.ResolveVersion("Step", "PackageA", "Package1"), Is.EqualTo("1.0.0"));
            Assert.That(resolver.ResolveVersion("Step", "PackageZ", "Package1"), Is.Null);
        }

        [Test]
        public void ShouldReturnDefaultWhenSet()
        {
            resolver.Default("2.91.0");

            Assert.That(resolver.ResolveVersion("Step", "PackageA", "Package1"), Is.EqualTo("2.91.0"));
            Assert.That(resolver.ResolveVersion("Step", "PackageB", "Package1"), Is.EqualTo("2.91.0"));
            Assert.That(resolver.ResolveVersion("Step", "PackageC", "Package1"), Is.EqualTo("2.91.0"));
        }

        [Test]
        public void ShouldParseConstraint()
        {
            resolver.Add("PackageA:Package1:1.0.0");
            resolver.Add("PackageB:Package1:1.0.0-alpha1");
            resolver.Add("PackageB=Package1=1.0.0-alpha1");

            Assert.That(resolver.ResolveVersion("Step", "PackageA", "Package1"), Is.EqualTo("1.0.0"));
            Assert.That(resolver.ResolveVersion("Step", "PackageB", "Package1"), Is.EqualTo("1.0.0-alpha1"));
        }

        [Test]
        public void ShouldParseWildCardConstraint()
        {
            resolver.Add("PackageA:Package2:2.0.0");
            resolver.Add("Step:Package2:3.0.0");
            resolver.Add("Step:4.0.0");
            resolver.Add("Package2:5.0.0");
            resolver.Add("PackageA:*:1.0.0");
            resolver.Add("*:Package1:1.0.0-alpha1");
            resolver.Add("*=Package1=1.0.0-alpha1");
            resolver.Add("*=*=1.0.0-beta");
            resolver.Add("*=6.0.0");

            // This is an exact match. We prioritise step names over package names
            Assert.That(resolver.ResolveVersion("Step", "PackageA", "Package2"), Is.EqualTo("3.0.0"));
            // This is an exact match to the step name
            Assert.That(resolver.ResolveVersion("Step", "PackageUnknown", "Package2"), Is.EqualTo("3.0.0"));
            // This is an exact match to the package name
            Assert.That(resolver.ResolveVersion("StepUnknown", "PackageA", "Package2"), Is.EqualTo("2.0.0"));
            // This is an exact match to the unnamed package version by step id
            Assert.That(resolver.ResolveVersion("Step", "PackageWhatever"), Is.EqualTo("4.0.0"));
            // This is an exact match to the unnamed package version by package id
            Assert.That(resolver.ResolveVersion("StepUnknown", "Package2"), Is.EqualTo("5.0.0"));
            // Unnamed packages also match the wildcard. In this case it is the wildcard for the unnamed packages
            Assert.That(resolver.ResolveVersion("StepUnknown", "PackageA"), Is.EqualTo("6.0.0"));
            // This will match the wildcard step but fixed package name version, because we treat the
            // package reference name as more specific
            Assert.That(resolver.ResolveVersion("Step", "PackageA", "Package1"), Is.EqualTo("1.0.0-alpha1"));
            // This will match the fixed step but wildcard package name version
            Assert.That(resolver.ResolveVersion("Step", "PackageA", "Unknown"), Is.EqualTo("1.0.0"));
            // This will also match the wildcard step and fixed package name version, because it is more
            // specific than the default
            Assert.That(resolver.ResolveVersion("Step", "PackageB", "Package1"), Is.EqualTo("1.0.0-alpha1"));
            // This will match the default (i.e. the double wildcard)
            Assert.That(resolver.ResolveVersion("StepWhatever", "PackageB", "PackageUnknown"), Is.EqualTo("1.0.0-beta"));
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
            Assert.That(resolver.ResolveVersion("StepName", "PackageId", "Package1"), Is.EqualTo("1.1.0"));
        }


        [Test]
        public void ShouldPreferPackageIdToDefault()
        {
            resolver.Default("1.0.0");
            resolver.Add("OtherStep", "Package1", "1.1.0");
            resolver.Add("PackageId", "Package1", "1.2.0");

            Assert.That(resolver.ResolveVersion("StepName", "PackageId", "Package1"), Is.EqualTo("1.2.0"));
        }
    }
}