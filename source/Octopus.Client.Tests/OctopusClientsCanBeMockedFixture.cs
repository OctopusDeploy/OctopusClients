using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace Octopus.Client.Tests
{
    public class OctopusClientsCanBeMockedFixture
    {
        [Test]
        public void WeShouldBeAbleToCreateAMockAsyncClient()
        {
            var client = Substitute.For<IOctopusAsyncClient>();
            client.Should().NotBeNull();

            Assert.True(false);
        }

        [Test]
        public void WeShouldBeAbleToCreateAMockClient()
        {
            var client = Substitute.For<IOctopusClient>();
            client.Should().NotBeNull();
        }
    }
}