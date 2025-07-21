using Octopus.Client.Model;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class BuildServerHeadersBambooTests : BuildServerHeadersBaseTests
    {
        protected override string EnvironmentVariableName => "bamboo_agentId";
        protected override string EnvironmentVariableValue => "123";
        internal override string ExpectedAutomationEnvironment => "Bamboo";
    }
}