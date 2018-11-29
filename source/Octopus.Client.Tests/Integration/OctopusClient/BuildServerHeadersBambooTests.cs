using Octopus.Client.Model;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class BuildServerHeadersBambooTests : BuildServerHeadersBaseTests
    {
        protected override string EnvironmentVariableName => OctopusCustomHeaders.EnvVar_Bamboo;
        protected override string EnvironmentVariableValue => "123";
        internal override BuildEnvironment ExpectedBuildEnvironment => BuildEnvironment.Bamboo;
    }
}