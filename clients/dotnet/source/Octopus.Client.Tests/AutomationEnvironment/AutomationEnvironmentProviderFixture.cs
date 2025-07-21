using NUnit.Framework;
using Octopus.Client.AutomationEnvironments;
using Octopus.Client.Model;

namespace Octo.Tests.Commands
{
    class AutomationEnvironmentProviderFixture
    {
        [Test]
        public void EnvironmentIsKnownIfBuildVariablesHaveValues()
        {
            foreach (var knownEnvironmentVariable in AutomationEnvironmentProvider.KnownEnvironmentVariables)
            {
                foreach (var variable in knownEnvironmentVariable.Value)
                {
                    AutomationEnvironmentProvider.environmentVariableReader = new ServerEnvironmentVariablesForTest(variable, "whatever value");
                    var provider = new AutomationEnvironmentProvider();

                    Assert.That(provider.DetermineAutomationEnvironment(), Is.Not.EqualTo(AutomationEnvironment.NoneOrUnknown));
                }
            }
        }

        [Test]
        public void EnvironmentIsUnknownIfBuildVariablesDontHaveValues()
        {
            AutomationEnvironmentProvider.environmentVariableReader = new ServerEnvironmentVariablesForTest(string.Empty, string.Empty);
            var provider = new AutomationEnvironmentProvider();

            Assert.That(provider.DetermineAutomationEnvironment(), Is.EqualTo(AutomationEnvironment.NoneOrUnknown));
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