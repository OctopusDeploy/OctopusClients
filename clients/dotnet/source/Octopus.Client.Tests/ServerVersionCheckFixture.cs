using FluentAssertions;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Validation;

namespace Octopus.Client.Tests
{
    public class ServerVersionCheckFixture
    {
        [Test]
        public void IsOlderThanClient_Development_False()
        {
            ServerVersionCheck.IsOlderThanClient("0.0.0-local", SemanticVersion.Parse("2019.11.0"))
                .Should().BeFalse();
        }

        [TestCase("2019.11.0")]
        [TestCase("2019.11.0-prerelease", Reason = "We should be ignoring the prerelease tag")]
        [TestCase("2019.11.1")]
        [TestCase("2019.12.0")]
        [TestCase("2020.0.0")]
        public void IsOlderThanClient_SameVersionOrAbove_False(string currentVersion)
        {
            var minimumVersion = "2019.11.0";
            ServerVersionCheck.IsOlderThanClient(currentVersion, SemanticVersion.Parse(minimumVersion))
                .Should().BeFalse();
        }
        
        [TestCase("2019.10.9")]
        [TestCase("2018.12.0")]
        public void IsOlderThanClient_LowerVersion_True(string currentVersion)
        {
            var minimumVersion = "2019.11.0";
            ServerVersionCheck.IsOlderThanClient(currentVersion, SemanticVersion.Parse(minimumVersion))
                .Should().BeTrue();
        }
    }
}