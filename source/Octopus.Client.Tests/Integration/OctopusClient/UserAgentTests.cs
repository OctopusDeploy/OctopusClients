using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy;
using NUnit.Framework;
using Octopus.Client.AutomationEnvironments;
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

                return Response.AsJson(new TestDto {UserAgentValue = userAgentHeaderValue})
                    .WithStatusCode(HttpStatusCode.OK);
            });
        }

        protected override void SetupEnvironmentVariables()
        {
            AutomationEnvironmentProvider.environmentVariableReader = new ServerEnvironmentVariablesForTest();
        }

        private class ServerEnvironmentVariablesForTest : IEnvironmentVariableReader
        {
            public string GetVariableValue(string name)
            {
                return null;
            }
        }

        protected override void CleanupEnvironmentVariables()
        {
            AutomationEnvironmentProvider.environmentVariableReader = new EnvironmentVariableReader();
        }


        [Test]
        public async Task AsyncClient_ShouldProvideUserAgent_WithNameAndVersion()
        {
            var response = await AsyncClient.Get<TestDto>(TestRootPath);
            response.UserAgentValue.Should().Be($"{ApiConstants.OctopusUserAgentProductName}/{GetType().GetSemanticVersion().ToNormalizedString()} NoneOrUnknown", "We should set the standard User-Agent header");
        }

#if SYNC_CLIENT
        [Test]
        public void SyncClient_ShouldProvideUserAgent_WithNameAndVersion()
        {
            var client = new Client.OctopusClient(new OctopusServerEndpoint(HostBaseUri + TestRootPath));
            var response = client.Get<TestDto>(TestRootPath);
            response.UserAgentValue.Should().Be($"{ApiConstants.OctopusUserAgentProductName}/{GetType().GetSemanticVersion().ToNormalizedString()} NoneOrUnknown", "We should set the standard User-Agent header");
        }
#endif

        public class TestDto
        {
            public string UserAgentValue { get; set; }
        }
    }
}