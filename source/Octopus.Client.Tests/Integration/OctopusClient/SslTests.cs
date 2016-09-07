#if HTTP_CLIENT_SUPPORTS_SSL_OPTIONS
using System;
using System.Net.Http;
using FluentAssertions;
using Nancy;
using NUnit.Framework;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class SslTests : HttpIntegrationTestBase
    {
        public SslTests()
        {
            Get(TestRootPath, p => Request.Url.IsSecure ? (object) "Data" : HttpStatusCode.UpgradeRequired);
        }

        [Test]
        public void InvalidSslCertificateIsRejected()
        {
            Action get = () => new Client.OctopusClient(new OctopusServerEndpoint(HostBaseSslUri + TestRootPath)).Get<string>("~/");
            get.ShouldThrow<HttpRequestException>().WithInnerMessage("A security error occurred");
        }

        [Test]
        public void InvalidSslCertificateIsIgnoredWhenTheOptionIsOn()
        {
            var result = new Client.OctopusClient(
                new OctopusServerEndpoint(HostBaseSslUri + TestRootPath),
                new OctopusClientOptions() { IgnoreSslErrors = true }
                )
                .Get<string>("~/");

            result.Should().Be("Data");
        }

    }
}
#endif