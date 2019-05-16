using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Diagnostics;
using Octopus.Client.AutomationEnvironments;

namespace Octo.Tests.Commands
{
    class ServiceMessagesTestFixture : ApiCommandFixtureBase
    {
        [Test]
        public void EnvironmentIsKnownIfBuildVariablesHaveValues()
        {
            foreach (var knownEnvironmentVariable in AutomationEnvironmentProvider.KnownEnvironmentVariables)
            {
                foreach (var variable in knownEnvironmentVariable.Value)
                {
                    AutomationEnvironmentProvider.environmentVariableReader = new ServerEnvironmentVariablesForTest(variable, "whatever value");

                    Log.EnableServiceMessages();

                    Assert.IsTrue(LogExtensions.IsKnownEnvironment());
                }
            }
        }

        [Test]
        public void EnvironmentIsUnknownIfBuildVariablesDontHaveValues()
        {
            AutomationEnvironmentProvider.environmentVariableReader = new ServerEnvironmentVariablesForTest(string.Empty, string.Empty);

            Log.EnableServiceMessages();

            Assert.IsFalse(LogExtensions.IsKnownEnvironment());
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
    }
}
