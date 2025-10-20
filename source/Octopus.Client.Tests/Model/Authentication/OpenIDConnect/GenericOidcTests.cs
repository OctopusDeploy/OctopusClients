using FluentAssertions;
using NUnit.Framework;
using Octopus.Client.Model.Authentication.OpenIDConnect.GenericOidc;

namespace Octopus.Client.Tests.Model.Authentication.OpenIDConnect
{
    public class GenericOidcTests
    {
        [Test]
        public void ConfigurationResource_HasCorrectId()
        {
            new GenericOidcConfigurationResource().Id.Should().Be(
                "authentication-generic-oidc", 
                "customers' database configuration will be stored against this value, changing it would cause that configuration to 'disappear'");
        }
    }
}
