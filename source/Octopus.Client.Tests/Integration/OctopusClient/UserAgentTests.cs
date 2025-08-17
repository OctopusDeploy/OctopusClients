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
            // Note: This test can fail when running locally with a mismatched git sha on the end of the version.
            // A typical user agent looks like this:
            //   OctopusClient-dotnet/1.0.0+8bc1b4a8856ac42a899919d3cbe29852d879132c (TestOS; x64) NoneOrUnknown
            // Because it is based on the assembly InformationalVersion, which microsoft started adding git sha's to around .NET 7
            // with a feature called SourceLink.
            //
            // If you run the tests locally, it is easy to end up with a version of Octopus.Client.dll (system under test) which was built
            // from an earlier commit, whereas the Test assembly is likely to be built with whatever HEAD is at the time.
            // If this happens, simply rebuild the projects in the `Clients` solution folder and the problem should go away
            var response = await AsyncClient.Get<TestDto>(TestRootPath);
            response.UserAgentValue.Should().Be($"{ApiConstants.OctopusUserAgentProductName}/{GetType().GetSemanticVersion().ToNormalizedString()} (TestOS; x64) NoneOrUnknown", "We should set the standard User-Agent header");
        }

        [Test]
        public void SyncClient_ShouldProvideUserAgent_WithNameAndVersion()
        {
            // As above can fail with mismatched git sha in version; rebuild folder if you need to
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
