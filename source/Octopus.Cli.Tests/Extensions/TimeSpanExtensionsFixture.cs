using System;
using NUnit.Framework;
using OctopusTools.Extensions;

namespace OctopusTools.Tests.Extensions
{
    [TestFixture]
    public class TimeSpanExtensionsFixture
    {
        [Test]
        public void FormattingTests()
        {
            Assert.That(new TimeSpan(3, 2, 5, 7).Friendly(), Is.EqualTo("3 days, 2h:5m:7s"));
            Assert.That(new TimeSpan(1, 2, 5, 7).Friendly(), Is.EqualTo("1 day, 2h:5m:7s"));
            Assert.That(new TimeSpan(0, 2, 5, 7).Friendly(), Is.EqualTo("2h:5m:7s"));
            Assert.That(new TimeSpan(0, 0, 5, 7).Friendly(), Is.EqualTo("5m:7s"));
            Assert.That(new TimeSpan(0, 0, 0, 7).Friendly(), Is.EqualTo("7s"));
            Assert.That(new TimeSpan(0, 0, 0, 0).Friendly(), Is.EqualTo("0s"));
            Assert.That(TimeSpan.FromSeconds(84).Friendly(), Is.EqualTo("1m:24s"));
        }
    }
}