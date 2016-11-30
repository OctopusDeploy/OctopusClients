using System;
using FluentAssertions;
using NUnit.Framework;

namespace Octopus.Client.Tests.Model.Versioning
{
    [TestFixture]
    public class SemanticVersionZeroQuirksFixture
    {
        [Test]
        public void NuGet_SemanticVersion_TreatsFourDigitVersionsAsInvalid()
        {
            Assert.Throws<ArgumentException>(() => NuGet.Versioning.SemanticVersion.Parse("1.0.0.0"));
        }

        [Test]
        public void NuGet_SemanticVersion_TreatsLeadingZerosAsInvalid()
        {
            Assert.Throws<ArgumentException>(() => NuGet.Versioning.SemanticVersion.Parse("2016.01.02"));
        }

        [Test]
        [TestCase("1.0.0.0")]
        [TestCase("1.0.0.999999")]
        public void NuGet_NuGetVersion_AcceptsFourDigitVersionsZeroInFourthDigitIsRetained(string version)
        {
            NuGet.Versioning.NuGetVersion.Parse(version).ToString().Should().Be(version);
        }

        [Test]
        public void NuGet_NuGetVersion_DropsLeadingZeros()
        {
            NuGet.Versioning.NuGetVersion.Parse("2016.01.02.0146").ToNormalizedString().Should().Be("2016.1.2.146");
        }

        [Test]
        [TestCase("1.0.0.0")]
        [TestCase("1.0.0.99999")]
        public void Octopus_SemanticVersion_ZeroInFourthDigitIsRetained(string version)
        {
            Client.Model.SemanticVersion.Parse(version).ToString().Should().Be(version);
        }

        [Test]
        [TestCase("2016.01.02.0146")]
        [TestCase("0001.0002.0003.0004")]
        public void Octopus_SemanticVersion_LeadingZerosAreRetained(string version)
        {
            Client.Model.SemanticVersion.Parse(version).ToString().Should().Be(version);
        }
    }
}