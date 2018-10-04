#if HTTP_CLIENT_SUPPORTS_SSL_OPTIONS
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy;
using NUnit.Framework;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class SslTests : HttpIntegrationTestBase
    {
        public SslTests()
            : base(UrlPathPrefixBehaviour.UseClassNameAsUrlPathPrefix)
        {
            Get(TestRootPath, p => Request.Url.IsSecure ? (object)"Data" : HttpStatusCode.UpgradeRequired);
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
                e.GetType().Name.Should().Be("AuthenticationException");
                e.Message.Should().Be("The remote certificate is invalid according to the validation procedure.");
            }
        }

        [Test]
        public async Task InvalidSslCertificateIsIgnoredWhenTheOptionIsOn()
        {
            try
            {
                var client = await OctopusAsyncClient.Create(
                    new OctopusServerEndpoint(HostBaseSslUri + TestRootPath),
                    new OctopusClientOptions() {IgnoreSslErrors = true}
                );
                var result = await client.Get<string>("~/");

                result.Should().Be("Data");
            }
            catch (Exception ex) when (ex.Message == "This platform does not support ignoring SSL certificate errors")
            {
                var os = RuntimeInformation.OSDescription;
                if (
                    os.StartsWith("Darwin") || // Mac
                    os.Contains(".el7") || // Cent OS
                    os.Contains("fc23") // Fedora 23
                )
                    return;

                throw;
            }
        }

    }
}
#endif