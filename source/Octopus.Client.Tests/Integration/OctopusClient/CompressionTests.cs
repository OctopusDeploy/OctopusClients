using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy;
using NUnit.Framework;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class CompressionTests : HttpIntegrationTestBase
    {
        private static readonly string[] ExpectedValues = { "deflate", "gzip" };
        
        public CompressionTests()
            : base(UrlPathPrefixBehaviour.UseClassNameAsUrlPathPrefix)
        {
            Get(TestRootPath, p =>
            {
                var acceptEncoding = Request.Headers.AcceptEncoding;

                return Response.AsJson(new TestDto { AcceptEncoding = acceptEncoding })
                    .WithStatusCode(HttpStatusCode.OK);
            });
        }

        [Test]
        public async Task AsyncClient_ShouldProvideAcceptEncoding()
        {
            var response = await AsyncClient.Get<TestDto>(TestRootPath);
            response.AcceptEncoding.Should().Contain(ExpectedValues);
        }

        [Test]
        public void SyncClient_ShouldProvideAcceptEncoding()
        {
            var client = new Client.OctopusClient(new OctopusServerEndpoint(HostBaseUri + TestRootPath));
            var response = client.Get<TestDto>(TestRootPath);
            response.AcceptEncoding.Should().Contain(ExpectedValues);
        }

        private class TestDto
        {
            public IEnumerable<string> AcceptEncoding { get; set; }
        }
    }
}