using Octopus.Client.Model;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class BuildServerHeadersTeamCityTests : BuildServerHeadersBaseTests
    {
        protected override string EnvironmentVariableName => "TEAMCITY_VERSION";
        protected override string EnvironmentVariableValue => "2018.1.3";
        internal override string ExpectedAutomationEnvironment => "TeamCity/2018.1.3";
    }
}