using Octopus.Client.Model;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class BuildServerHeadersTeamCityTests : BuildServerHeadersBaseTests
    {
        protected override string EnvironmentVariableName => OctopusCustomHeaders.EnvVar_TeamCity;
        protected override string EnvironmentVariableValue => "2018.1.3";
        internal override BuildEnvironment ExpectedBuildEnvironment => BuildEnvironment.TeamCity;
    }
}