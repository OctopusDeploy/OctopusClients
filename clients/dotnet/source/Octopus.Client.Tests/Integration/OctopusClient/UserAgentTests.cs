using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy;
using NSubstitute;
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
            OctopusCustomHeaders.environmentHelper = new TestEnvironmentHelper();
            AutomationEnvironmentProvider.environmentVariableReader = new ServerEnvironmentVariablesForTest();
        }

        private class TestEnvironmentHelper : IEnvironmentHelper
        {
            public string[] SafelyGetEnvironmentInformation()
            {
                return new[] {"TestOS", "x64"};
            }
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
            OctopusCustomHeaders.environmentHelper = new EnvironmentHelper();
            AutomationEnvironmentProvider.environmentVariableReader = new EnvironmentVariableReader();
        }


        [Test]
        public async Task AsyncClient_ShouldProvideUserAgent_WithNameAndVersion()
        {
            var response = await AsyncClient.Get<TestDto>(TestRootPath);
            response.UserAgentValue.Should().Be($"{ApiConstants.OctopusUserAgentProductName}/{GetType().GetSemanticVersion().ToNormalizedString()} (TestOS; x64) NoneOrUnknown", "We should set the standard User-Agent header");
        }

        [Test]
        public void SyncClient_ShouldProvideUserAgent_WithNameAndVersion()
        {
            var client = new Client.OctopusClient(new OctopusServerEndpoint(HostBaseUri + TestRootPath));
            var response = client.Get<TestDto>(TestRootPath);
            response.UserAgentValue.Should().Be($"{ApiConstants.OctopusUserAgentProductName}/{GetType().GetSemanticVersion().ToNormalizedString()} (TestOS; x64) NoneOrUnknown", "We should set the standard User-Agent header");
        }

        public class TestDto
        {
            public string UserAgentValue { get; set; }
        }
    }
}