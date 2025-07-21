using System;
using NUnit.Framework;

namespace Octopus.Client.Tests
{
    [TestFixture]
    public class DefaultLinkResolverFixture
    {
        [Test]
        [TestCase("~/", "http://octopus/")]
        [TestCase("~/api", "http://octopus/api")]
        [TestCase("~/api/foo", "http://octopus/api/foo")]
        [TestCase("~/api/foo/bar", "http://octopus/api/foo/bar")]
        public void ShouldResolveWhenNoSuffixGiven(string link, string expectedResult)
        {
            var resolver = new DefaultLinkResolver(new Uri("http://octopus/"));
            Assert.That(resolver.Resolve(link).ToString(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase("~/", "http://octopus/")]
        [TestCase("~/api", "http://octopus/api")]
        [TestCase("~/api/foo", "http://octopus/api/foo")]
        [TestCase("~/api/foo/bar", "http://octopus/api/foo/bar")]
        public void ShouldResolveWhenApiSuffixGiven(string link, string expectedResult)
        {
            var resolver = new DefaultLinkResolver(new Uri("http://octopus/api"));
            Assert.That(resolver.Resolve(link).ToString(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase("~/", "http://octopus/vdir/")]
        [TestCase("~/api", "http://octopus/vdir/api")]
        [TestCase("~/api/foo", "http://octopus/vdir/api/foo")]
        [TestCase("~/api/foo/bar", "http://octopus/vdir/api/foo/bar")]
        public void ShouldResolveWhenVirtualDirectorySuffixGiven(string link, string expectedResult)
        {
            var resolver = new DefaultLinkResolver(new Uri("http://octopus/vdir"));
            Assert.That(resolver.Resolve(link).ToString(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase("~/", "http://octopus/vdir/")]
        [TestCase("~/api", "http://octopus/vdir/api")]
        [TestCase("~/api/foo", "http://octopus/vdir/api/foo")]
        [TestCase("~/api/foo/bar", "http://octopus/vdir/api/foo/bar")]
        public void ShouldResolveWhenVirtualDirectoryApiSuffixGiven(string link, string expectedResult)
        {
            var resolver = new DefaultLinkResolver(new Uri("http://octopus/vdir/api"));
            Assert.That(resolver.Resolve(link).ToString(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase("~/", "http://octopus/vdir1/vdir2/")]
        [TestCase("~/api", "http://octopus/vdir1/vdir2/api")]
        [TestCase("~/api/foo", "http://octopus/vdir1/vdir2/api/foo")]
        [TestCase("~/api/foo/bar", "http://octopus/vdir1/vdir2/api/foo/bar")]
        public void ShouldResolveWhenNestedVirtualDirectorySuffixGiven(string link, string expectedResult)
        {
            var resolver = new DefaultLinkResolver(new Uri("http://octopus/vdir1/vdir2"));
            Assert.That(resolver.Resolve(link).ToString(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase("~/", "http://octopus/vdir1/vdir2/")]
        [TestCase("~/api", "http://octopus/vdir1/vdir2/api")]
        [TestCase("~/api/foo", "http://octopus/vdir1/vdir2/api/foo")]
        [TestCase("~/api/foo/bar", "http://octopus/vdir1/vdir2/api/foo/bar")]
        public void ShouldResolveWhenNestedVirtualDirectoryApiSuffixGiven(string link, string expectedResult)
        {
            var resolver = new DefaultLinkResolver(new Uri("http://octopus/vdir1/vdir2/api"));
            Assert.That(resolver.Resolve(link).ToString(), Is.EqualTo(expectedResult));
        }
    }
}