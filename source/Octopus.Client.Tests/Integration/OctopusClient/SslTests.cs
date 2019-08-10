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
            OctopusAsyncRepository.SecondsToWaitForServerToStart = 2;
            try
            {
                await OctopusAsyncClient.Create(new OctopusServerEndpoint(HostBaseSslUri + TestRootPath)).ConfigureAwait(false);
                Assert.Fail("Exception expected");
            }
            catch (Exception ex)
            {
                var e = ex.InnerException?.InnerException;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    e.GetType().Name.Should().BeOneOf("WinHttpException", "AuthenticationException");
                    e.Message.Should().BeOneOf("A security error occurred",
                        "The remote certificate is invalid according to the validation procedure.");
                }
                else
                {
                    e.GetType().Name.Should().Be("CurlException");
                    e.Message.Should().Be("Peer certificate cannot be authenticated with given CA certificates");
                }
            }
            OctopusAsyncRepository.SecondsToWaitForServerToStart = 60;
        }

        [Test]
        public async Task InvalidSslCertificateIsIgnoredWhenTheOptionIsOn()
        {
            try
            {
                var client = await OctopusAsyncClient.Create(
                    new OctopusServerEndpoint(HostBaseSslUri + TestRootPath),
                    new OctopusClientOptions() {IgnoreSslErrors = true}
                ).ConfigureAwait(false);
                var result = await client.Get<string>("~/").ConfigureAwait(false);

                result.Should().Be("Data");
            }
            catch (Exception ex) when (ex.Message == "This platform does not support ignoring SSL certificate errors")
            {
                Console.WriteLine($"This test is running on '{RuntimeInformation.OSDescription}'");
                var os = RuntimeInformation.OSDescription;
                if (
                    os.StartsWith("Darwin") || // Mac
                    os.Contains(".el7") || // Cent OS
                    os.Contains("fc23") // Fedora 23
                )
                {
                    Assert.Inconclusive($"This test is known not to work on platform '{RuntimeInformation.OSDescription}'");
                }

                throw;
            }
        }

    }
}
#endif