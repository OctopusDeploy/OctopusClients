using System;
using System.Threading.Tasks;
using FluentAssertions;
using Nancy;
using NUnit.Framework;
using Octopus.Client.AutomationEnvironments;
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
                var automationContext = Request.Headers.UserAgent.Replace("; ", ";").Split(' ')[2];

                return Response.AsJson(new TestDto { AutomationContext = automationContext })
                    .WithStatusCode(HttpStatusCode.OK);
            });
        }

        protected abstract string EnvironmentVariableName { get; }
        protected abstract string EnvironmentVariableValue { get; }
        internal abstract string ExpectedAutomationEnvironment { get; }

        protected override void SetupEnvironmentVariables()
        {
            OctopusCustomHeaders.environmentHelper = new TestEnvironmentHelper();
            AutomationEnvironmentProvider.environmentVariableReader = new ServerEnvironmentVariablesForTest(EnvironmentVariableName, EnvironmentVariableValue);
        }

        private class TestEnvironmentHelper : IEnvironmentHelper
        {
            public string[] SafelyGetEnvironmentInformation()
            {
                return new[] { "TestOS", "x64" };
            }
        }

        private class ServerEnvironmentVariablesForTest : IEnvironmentVariableReader
        {
            public ServerEnvironmentVariablesForTest(string environmentVariableName, string environmentVariableValue)
            {
                EnvironmentVariableName = environmentVariableName;
                EnvironmentVariableValue = environmentVariableValue;
            }

            private string EnvironmentVariableName { get; set; }
            private string EnvironmentVariableValue { get; set; }
            public string GetVariableValue(string name)
            {
                return name == EnvironmentVariableName ?
                    EnvironmentVariableValue :
                    null;
            }
        }

        protected override void CleanupEnvironmentVariables()
        {
            OctopusCustomHeaders.environmentHelper = new EnvironmentHelper();
            AutomationEnvironmentProvider.environmentVariableReader = new EnvironmentVariableReader();
        }

        [Test]
        public async Task AsyncClient_ShouldProvideBuildServer_WithCorrectValue()
        {
            var response = await AsyncClient.Get<TestDto>(TestRootPath);
            response.AutomationContext.Should().Be(ExpectedAutomationEnvironment.ToString(), $"We should set the User-Agent header to have {ExpectedAutomationEnvironment} when {EnvironmentVariableName} is set");
        }

        [Test]
        public void SyncClient_ShouldProvideBuildServer_WithCorrectValue()
        {
            var client = new Client.OctopusClient(new OctopusServerEndpoint(HostBaseUri + TestRootPath));
            var response = client.Get<TestDto>(TestRootPath);
            response.AutomationContext.Should().Be(ExpectedAutomationEnvironment.ToString(), $"We should set the User-Agent header to have {ExpectedAutomationEnvironment} when {EnvironmentVariableName} is set");
        }

        public class TestDto
        {
            public string AutomationContext { get; set; }
        }
    }
}