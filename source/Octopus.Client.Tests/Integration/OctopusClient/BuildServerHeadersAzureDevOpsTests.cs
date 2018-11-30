using Octopus.Client.Model;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class BuildServerHeadersAzureDevOpsTests : BuildServerHeadersBaseTests
    {
        protected override string EnvironmentVariableName => "TF_BUILD";
        protected override string EnvironmentVariableValue => "True";
        internal override AutomationEnvironment ExpectedAutomationEnvironment => AutomationEnvironment.AzureDevOps;
    }
}