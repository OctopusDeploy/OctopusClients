using System;
using NUnit.Framework;
using OctopusTools.Commands;
using OctopusTools.Diagnostics;

namespace OctopusTools.Tests.Commands
{
    [TestFixture]
    public class PackageVersionResolverFixture
    {
        PackageVersionResolver resolver;

        [SetUp]
        public void SetUp()
        {
            resolver = new PackageVersionResolver(Logger.Default);
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
            Assert.Throws<ArgumentException>(() => resolver.Add(":"));
            Assert.Throws<ArgumentException>(() => resolver.Add("="));
            Assert.Throws<ArgumentException>(() => resolver.Add(":1.0.0"));
            Assert.Throws<ArgumentException>(() => resolver.Add("=1.0.0"));
            Assert.Throws<ArgumentException>(() => resolver.Add("PackageA:"));
            Assert.Throws<ArgumentException>(() => resolver.Add("PackageA:1.FRED.9"));
            Assert.Throws<ArgumentException>(() => resolver.Add("PackageA=1.FRED.9"));
        }
    }
}
