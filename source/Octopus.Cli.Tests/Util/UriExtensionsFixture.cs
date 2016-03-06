using System;
using NUnit.Framework;
using Octopus.Cli.Util;

namespace Octopus.Cli.Tests.Util
{
    [TestFixture]
    public class UriExtensionsFixture
    {
        [Test]
        public void ShouldAppendSuffixIfThereIsNoOverlap()
        {
            var result = new Uri("http://www.mysite.com").EnsureEndsWith("suffix");
            
            Assert.AreEqual(result.ToString(), "http://www.mysite.com/suffix");
        }

        [Test]
        public void ShouldRemoveAnyOverlapBetweenBaseAddresAndSuffix()
        {
            var result = new Uri("http://www.mysite.com/virtual").EnsureEndsWith("/virtual/suffix");

            Assert.AreEqual(result.ToString(), "http://www.mysite.com/virtual/suffix");
        }
    }
}
