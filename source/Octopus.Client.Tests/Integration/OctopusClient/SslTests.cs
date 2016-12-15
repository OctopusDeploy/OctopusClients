#if HTTP_CLIENT_SUPPORTS_SSL_OPTIONS
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
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
        public async Task InvalidSslCertificateIsRejected()
        {
            try
            {
                await OctopusAsyncClient.Create(new OctopusServerEndpoint(HostBaseSslUri + TestRootPath));
                Assert.Fail("Exception expected");
            }
            catch (Exception ex)
            {
                var e = ex.InnerException?.InnerException;
                e.GetType().Name.Should().Be("WinHttpException");
                e.Message.Should().Be("A security error occurred");
            }
        }

        [Test]
        public async Task InvalidSslCertificateIsIgnoredWhenTheOptionIsOn()
        {
            var client = await OctopusAsyncClient.Create(
                new OctopusServerEndpoint(HostBaseSslUri + TestRootPath),
                new OctopusClientOptions() { IgnoreSslErrors = true }
            );
            var result = await client.Get<string>("~/");

            result.Should().Be("Data");
        }

    }
}
#endif