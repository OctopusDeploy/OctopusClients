using System.Collections.Generic;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Model.Versioning;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace Octopus.Client.Tests.Model.Versioning
{
    [TestFixture]
    public class VersionComparerFixture
    {
        
        [Test]
        [TestCase("1.0.0", "1.0.0")]
        [TestCase("1.0.0-BETA", "1.0.0-beta")]
        [TestCase("1.0.0-BETA+AA", "1.0.0-beta+aa")]
        [TestCase("1.0.0-BETA+AA", "1.0.0-beta+aa")]
        [TestCase("1.0.0-BETA.X.y.5.77.0+AA", "1.0.0-beta.x.y.5.77.0+aa")]
        public void VersionComparisonDefaultEqual(string version1, string version2)
        {
            // Arrange & Act
            var match = Equals(VersionComparer.Default, version1, version2);

            // Assert
            Assert.True(match);
        }

        [TestCase("0.0.0", "1.0.0")]
        [TestCase("1.1.0", "1.0.0")]
        [TestCase("1.0.1", "1.0.0")]
        [TestCase("1.0.1", "1.0.0")]
        [TestCase("1.0.0-BETA", "1.0.0-beta2")]
        [TestCase("1.0.0+AA", "1.0.0-beta+aa")]
        [TestCase("1.0.0-BETA+AA", "1.0.0-beta")]
        [TestCase("1.0.0-BETA.X.y.5.77.0+AA", "1.0.0-beta.x.y.5.79.0+aa")]
        [TestCase("1.0.0+AA", "1.0.0+BB")]
        public void VersionComparisonDefaultNotEqual(string version1, string version2)
        {
            // Arrange & Act
            var match = !Equals(version1, version2);

            // Assert
            Assert.True(match);
        }

        [TestCase("0.0.0", "1.0.0")]
        [TestCase("1.0.0", "1.1.0")]
        [TestCase("1.0.0", "1.0.1")]
        [TestCase("1.999.9999", "2.1.1")]
        [TestCase("1.0.0-BETA", "1.0.0-beta2")]
        [TestCase("1.0.0-beta+AA", "1.0.0+aa")]
        [TestCase("1.0.0-BETA", "1.0.0-beta.1+AA")]
        [TestCase("1.0.0-BETA.X.y.5.77.0+AA", "1.0.0-beta.x.y.5.79.0+aa")]
        [TestCase("1.0.0-BETA.X.y.5.79.0+AA", "1.0.0-beta.x.y.5.790.0+abc")]
        public void VersionComparisonDefaultLess(string version1, string version2)
        {
            // Arrange & Act
            var result = Compare(VersionComparer.Default, version1, version2);

            // Assert
            Assert.True(result < 0);
        }

        static int Compare(IVersionComparer comparer, string version1, string version2)
        {
            // Act
            var x = CompareOneWay(comparer, version1, version2);
            var y = CompareOneWay(comparer, version2, version1) * -1;

            // Assert
            Assert.AreEqual(x,y);

            return x;
        }

        static int CompareOneWay(IVersionComparer comparer, string version1, string version2)
        {
            // Arrange
            var a = SemanticVersion.Parse(version1);
            var b = SemanticVersion.Parse(version2);
            var c = StrictSemanticVersion.Parse(version1);
            var d = StrictSemanticVersion.Parse(version2);

            // Act
            var results = new List<int>();
            results.Add(comparer.Compare(a, b));
            results.Add(comparer.Compare(a, d));
            results.Add(comparer.Compare(c, b));
            results.Add(comparer.Compare(c, d));

            // Assert
            Assert.True(results.FindAll(x => x == results[0]).Count == results.Count);

            return results[0];
        }

        static bool Equals(IVersionComparer comparer, string version1, string version2)
        {
            return EqualsOneWay(comparer, version1, version2) && EqualsOneWay(comparer, version2, version1);
        }

        static bool EqualsOneWay(IVersionComparer comparer, string version1, string version2)
        {
            // Arrange
            var a = SemanticVersion.Parse(version1);
            var b = SemanticVersion.Parse(version2);
            StrictSemanticVersion c = SemanticVersion.Parse(version1);
            StrictSemanticVersion d = SemanticVersion.Parse(version2);

            // Act
            var match = Compare(comparer, version1, version2) == 0;
            match &= comparer.Equals(a, b);
            match &= comparer.Equals(a, d);
            match &= comparer.Equals(c, d);
            match &= comparer.Equals(c, b);

            return match;
        }
    }
}