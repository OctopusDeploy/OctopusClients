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
                var automationContext = Request.Headers.UserAgent.Split(' ')[1];

                return Response.AsJson(new TestDto { AutomationContext = automationContext })
                    .WithStatusCode(HttpStatusCode.OK);
            });
        }

        protected abstract string EnvironmentVariableName { get; }
        protected abstract string EnvironmentVariableValue { get; }
        internal abstract BuildEnvironment ExpectedBuildEnvironment { get; }

        protected override void SetupEnvironmentVariables()
        {
            if (!string.IsNullOrWhiteSpace(EnvironmentVariableName))
            {
                OctopusCustomHeaders.GetEnvironmentVariable = variableName => variableName == EnvironmentVariableName ? EnvironmentVariableValue : null;
            }
        }

        protected override void CleanupEnvironmentVariables()
        {
            if (!string.IsNullOrWhiteSpace(EnvironmentVariableName))
            {
                OctopusCustomHeaders.GetEnvironmentVariable = Environment.GetEnvironmentVariable;
            }
        }

        [Test]
        public async Task AsyncClient_ShouldProvideBuildServer_WithCorrectValue()
        {
            var response = await AsyncClient.Get<TestDto>(TestRootPath);
            response.AutomationContext.Should().Be(ExpectedBuildEnvironment.ToString(), $"We should set the User-Agent header to have {ExpectedBuildEnvironment} when {EnvironmentVariableName} is set");
        }

#if SYNC_CLIENT
        [Test]
        public void SyncClient_ShouldProvideBuildServer_WithCorrectValue()
        {
            var client = new Client.OctopusClient(new OctopusServerEndpoint(HostBaseUri + TestRootPath));
            var response = client.Get<TestDto>(TestRootPath);
            response.AutomationContext.Should().Be(ExpectedBuildEnvironment.ToString(), $"We should set the User-Agent header to have {ExpectedBuildEnvironment} when {EnvironmentVariableName} is set");
        }
#endif

        public class TestDto
        {
            public string AutomationContext { get; set; }
        }
    }
}