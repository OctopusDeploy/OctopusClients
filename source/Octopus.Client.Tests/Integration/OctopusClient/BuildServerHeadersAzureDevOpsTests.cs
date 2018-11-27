using Octopus.Client.Model;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class BuildServerHeadersAzureDevOpsTests : BuildServerHeadersBaseTests
    {
        protected override string EnvironmentVariableName => OctopusCustomHeaders.EnvVar_AzureDevOps;
        protected override string EnvironmentVariableValue => "True";
        internal override BuildServer ExpectedBuildServer => BuildServer.AzureDevOps;
    }
}