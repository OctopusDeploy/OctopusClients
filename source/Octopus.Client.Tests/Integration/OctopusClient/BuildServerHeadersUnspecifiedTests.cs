using Octopus.Client.Model;

namespace Octopus.Client.Tests.Integration.OctopusClient
{
    public class BuildServerHeadersUnspecifiedTests : BuildServerHeadersBaseTests
    {
        protected override string EnvironmentVariableName => string.Empty;
        protected override string EnvironmentVariableValue => string.Empty;
        internal override string ExpectedAutomationEnvironment => "NoneOrUnknown";
    }
}