using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy;
using NUnit.Framework;
using Octopus.Client.Model;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public abstract class BuildServerHeadersBaseTests : HttpIntegrationTestBase
    {
        protected BuildServerHeadersBaseTests()
            : base(UrlPathPrefixBehaviour.UseClassNameAsUrlPathPrefix)
        {
            Get(TestRootPath, p =>
            {
                var buildServerValue = Request.Headers[ApiConstants.BuildServerHeaderName]?.FirstOrDefault();

                return Response.AsJson(new TestDto { BuildServerValue = buildServerValue })
                    .WithStatusCode(HttpStatusCode.OK);
            });
        }

        protected abstract string EnvironmentVariableName { get; }
        protected abstract string EnvironmentVariableValue { get; }
        internal abstract BuildServer ExpectedBuildServer { get; }

        protected override void SetupEnvironmentVariables()
        {
            foreach (var envVar in OctopusCustomHeaders.BuildServerEnvVars)
            {
                Environment.SetEnvironmentVariable(envVar, string.Empty);
            }

            if (!string.IsNullOrWhiteSpace(EnvironmentVariableName))
            {
                Environment.SetEnvironmentVariable(EnvironmentVariableName, EnvironmentVariableValue);
            }
        }

        [Test]
        public async Task AsyncClient_ShouldProvideBuildServer_WithCorrectValue()
        {
            var response = await AsyncClient.Get<TestDto>(TestRootPath);
            response.BuildServerValue.Should().Be(ExpectedBuildServer.ToString(), $"We should set the {ExpectedBuildServer} X-Octopus-BuildServer header");
        }

#if SYNC_CLIENT
        [Test]
        public void SyncClient_ShouldProvideBuildServer_WithCorrectValue()
        {
            var client = new Client.OctopusClient(new OctopusServerEndpoint(HostBaseUri + TestRootPath));
            var response = client.Get<TestDto>(TestRootPath);
            response.BuildServerValue.Should().Be(ExpectedBuildServer.ToString(), $"We should set the {ExpectedBuildServer} X-Octopus-BuildServer header");
        }
#endif

        public class TestDto
        {
            public string BuildServerValue { get; set; }
        }
    }
}