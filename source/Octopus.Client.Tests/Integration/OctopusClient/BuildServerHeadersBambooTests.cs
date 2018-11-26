using Octopus.Client.Model;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class BuildServerHeadersBambooTests : BuildServerHeadersBaseTests
    {
        protected override string EnvironmentVariableName => OctopusCustomHeaders.EnvVar_Bamboo;
        protected override string EnvironmentVariableValue => "123";
        protected override BuildServer ExpectedBuildServer => BuildServer.Bamboo;
    }
}