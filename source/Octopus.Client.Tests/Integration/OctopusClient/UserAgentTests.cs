using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy;
using NUnit.Framework;
using Octopus.Client.Model;
using Octopus.Client.Extensions;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class UserAgentTests : HttpIntegrationTestBase
    {
        public UserAgentTests()
            : base(UrlPathPrefixBehaviour.UseClassNameAsUrlPathPrefix)
        {
            Get(TestRootPath, p =>
            {
                var userAgentHeaderValue = Request.Headers["User-Agent"]?.FirstOrDefault();
                var customUserAgentHeaderValue = Request.Headers[ApiConstants.OctopusUserAgentHeaderName]?.FirstOrDefault();

                return Response.AsJson(new TestDto {UserAgentValue = userAgentHeaderValue, CustomUserAgentValue = customUserAgentHeaderValue})
                    .WithStatusCode(HttpStatusCode.OK);
            });
        }

        [Test]
        public async Task AsyncClient_ShouldProvideUserAgent_WithNameAndVersion()
        {
            var response = await AsyncClient.Get<TestDto>(TestRootPath);
            response.UserAgentValue.Should().Be($"OctopusClient-dotnet/{GetType().GetSemanticVersion().ToNormalizedString()}", "We should set the standard User-Agent header");
            response.CustomUserAgentValue.Should().Be($"OctopusClient-dotnet/{GetType().GetSemanticVersion().ToNormalizedString()}", $"We should set our custom {ApiConstants.OctopusUserAgentHeaderName} header");
        }

#if SYNC_CLIENT
        [Test]
        public void SyncClient_ShouldProvideUserAgent_WithNameAndVersion()
        {
            var client = new Client.OctopusClient(new OctopusServerEndpoint(HostBaseUri + TestRootPath));
            var response = client.Get<TestDto>(TestRootPath);
            response.UserAgentValue.Should().Be($"OctopusClient-dotnet/{GetType().GetSemanticVersion().ToNormalizedString()}", "We should set the standard User-Agent header");
            response.CustomUserAgentValue.Should().Be($"OctopusClient-dotnet/{GetType().GetSemanticVersion().ToNormalizedString()}", $"We should set our custom {ApiConstants.OctopusUserAgentHeaderName} header");
        }
#endif

        public class TestDto
        {
            public string UserAgentValue { get; set; }
            public string CustomUserAgentValue { get; set; }
        }
    }
}