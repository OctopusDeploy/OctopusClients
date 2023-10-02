using System.Linq;
using NUnit.Framework;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.Model
{
    [TestFixture]
    public class SemanticVersionMaskFixture
    {
        [Test]
        [TestCase("1.0", false)]
        [TestCase("1.0.0", false)]
        [TestCase("1.0.0.0", false)]
        [TestCase("1.0.0.0-foo", false)]
        [TestCase("1.0.0-foo", false)]
        [TestCase("1.0-foo", false)]
        [TestCase("1.-i", false)]
        [TestCase("1.1.2-i", false)]
        [TestCase("127.0.0.1-localhost", false)]
        [TestCase("1.i", true)]
        [TestCase("1.0.i", true)]
        [TestCase("1.0.0.i", true)]
        [TestCase("1.0.c.i", true)]
        [TestCase("i.i", true)]
        [TestCase("c.c", true)]
        [TestCase("i.i.i", true)]
        [TestCase("i.i.0", true)]
        [TestCase("c.c.0", true)]
        [TestCase("c.c.i.0", true)]
        [TestCase("c.c.c.i", true)]
        [TestCase("i.i.i.i", true)]
        [TestCase("127.c.0.i-localhost", true)]
        [TestCase("1.0.0-alpha.1", false)]
        [TestCase("1.0.0-alpha.beta.1", false)]
        [TestCase("1.0.0-alpha.i", true)]
        [TestCase("1.0.0-alpha.beta9.i", true)]
        [TestCase("1.0.c-alpha.beta9.i", true)]
        [TestCase("1.0.0-alpha.i+foo", false)]
        public void ShouldBeMask(string version, bool valid)
        {
            var isMask = SemanticVersionMask.IsMask(version);
            Assert.That(isMask, Is.EqualTo(valid));
        }

        [Test]
        [TestCase("0.i", "0.0", "0.1")]
        [TestCase("1.i", "1.0", "1.1")]
        [TestCase("1.1.i", "1.1.1234", "1.1.1235", Description = "Increment on build number increases build by one")]
        [TestCase("c.c.i", "1.0.1", "1.0.2", Description = "Mixed substitutions")]
        [TestCase("1.i", "1.1.5", "1.2.0", Description = "Reset missing sub-numbers to zero")]
        [TestCase("1.i.1", "1.1.5", "1.2.1", Description = "Allows sub-numbers")]
        [TestCase("1.c.i", "1.1", "1.1.1", Description = "Increment on non-existant sub-number assumes previous zero")]
        [TestCase("1.c.i", "1.1.5", "1.1.6", Description = "Increment on current sub-number adds one")]
        [TestCase("1.i.i", "1.1.5", "1.2.0", Description = "Increment on increment sub-number resets to zero")]
        [TestCase("1.i.c", "1.1.5", "1.2.0", Description = "Current on increment sub-number resets to zero")]
        [TestCase("2.19.i-channel", "2.19.30", "2.19.31-channel", Description = "Tag in mask preserved")]
        [TestCase("2.19.i-channel", "2.19.30-baz", "2.19.31-channel", Description = "Tag in mask overrides current tag")]
        [TestCase("2.19.i", "2.19.30-baz", "2.19.31", Description = "Tag on current ignored if not in mask")]
        [TestCase("1.2.c-alpha.i", "1.2.3-alpha.4", "1.2.3-alpha.5", Description = "Increments pre-release")]
        [TestCase("1.2.0-alpha.i", "1.2.3", "1.2.0-alpha.1", Description = "Increment on nonexistent pre-release identifier assumes previous zero")]
        [TestCase("1.2.0-alpha.c.i.i", "1.2.0-alpha.2.3.4", "1.2.0-alpha.2.4.0", Description = "Increment on increment pre-release identifier resets to zero")]
        public void ShouldApplyMask(string mask, string current, string expected)
        {
            var currentVersion = SemanticVersion.Parse(current);
            var resultVersion = SemanticVersionMask.ApplyMask(mask, currentVersion);
            var received = resultVersion.ToString();

            Assert.That(received, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("1.c.i", "1.0.0", Description = "Current and increment substitute treated as zero")]
        [TestCase("c.i", "0.0", Description = "Major will start from zero")]
        [TestCase("c.i.1-foo", "0.0.1-foo", Description = "Tags are preserved")]
        public void ShouldCreateZeroedVersionWhenNoCurrentPresent(string mask, string expected)
        {
            var resultVersion = SemanticVersionMask.ApplyMask(mask, null);
            var received = resultVersion.ToString();

            Assert.That(received, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("2.2.i", new string[] {"1.1.1", "2.2.1", "2.2.3", "2.2"}, "2.2.3", Description = "Trailing substitute")]
        [TestCase("2.2.c", new string[] { "1.1.1", "2.2.1", "2.2.3", "2.2" }, "2.2.3", Description = "Substituion 'current' character")]
        [TestCase("2.3.i", new string[] {"1.1.1", "2.2.1", "2.2.3", "2.3"}, "2.3", Description = "Version without build")]
        [TestCase("2.i.i", new string[] { "1.1.1", "2.2.1", "2.2.3", "2.3" }, "2.3", Description = "Multiple substitutions")]
        [TestCase("1.i.i", new string[] {"1.1.1", "2.2.1", "2.2.3", "2.3"}, "1.1.1", Description = "Non global max")]
        [TestCase("2.19.i-channel", new string[] {"2.19.30-baz", "2.19.31", "2.20.0"}, "2.19.31", Description = "Tag in mask")]
        [TestCase("2.19.i", new string[] { "2.19.30-baz", "2.19.29", "2.20.0" }, "2.19.30-baz", Description = "Tag in match")]
        [TestCase("4.i.1-tagx", new string[] { "4.0.1-tagx", "4.1", "5.0.0" }, "4.1", Description = "Tag in match")]
        [TestCase("2.i", new string[] { "2.19.2" }, "2.19.2")]
        public void ShouldGetTheCorrectLatestVersion(string mask, string[] versionList, string expected)
        {
            var versions = versionList.Select(version => SemanticVersion.Parse(version)).ToList();
            var currentVersion = SemanticVersionMask.GetLatestMaskedVersion(mask, versions);

            Assert.AreEqual(currentVersion.ToString(), expected);
        }

        [Test]
        [TestCase("0.1.1", new string[] {"1.1.1", "2.2.1", "2.2.3", "2.3"})]
        [TestCase("4.1.1", new string[] {"1.1.1", "2.2.1", "2.2.3", "2.3"})]
        [TestCase("1.2.i", new string[] { "1.1.1", "2.2.1", "2.2.3", "2.3" })]
        [TestCase("2.2.4.i", new string[] { "1.1.1", "2.2.1", "2.2.3", "2.3" })]
        [TestCase("2.19", new string[] { "2.19.2" })]
        [TestCase("2.2.4.i", new string[] { })]
        public void ShouldNotMatchAnyVersions(string mask, string[] versionList)
        {
            var versions = versionList.Select(version => SemanticVersion.Parse(version)).ToList();
            var currentVersion = SemanticVersionMask.GetLatestMaskedVersion(mask, versions);

            Assert.IsNull(currentVersion);
        }

    }
}