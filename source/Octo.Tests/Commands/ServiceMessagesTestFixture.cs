using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Diagnostics;

namespace Octo.Tests.Commands
{
    class ServiceMessagesTestFixture : ApiCommandFixtureBase
    {
        IEnvironmentVariableReader envVariableGetter;

        [Test]
        public void EnvironmentIsKnownIfBuildVariablesHaveValues()
        {
            foreach (var knownEnvironmentVariable in LogExtensions.KnownEnvironmentVariables)
            {
                foreach (var variable in knownEnvironmentVariable.Value)
                {
                    envVariableGetter = Substitute.For<IEnvironmentVariableReader>();

                    envVariableGetter.GetVariableValue(variable).Returns("whatever value");

                    LogExtensions.environmentVariableReader = envVariableGetter;

                    LogExtensions.EnableServiceMessages(Log);

                    Assert.IsTrue(LogExtensions.IsKnownEnvironment());
                }
            }
        }

        [Test]
        public void EnvironmentIsUnknownIfBuildVariablesDontHaveValues()
        {
            envVariableGetter = Substitute.For<IEnvironmentVariableReader>();

            foreach (var knownEnvironmentVariable in LogExtensions.KnownEnvironmentVariables)
            {
                foreach (var variable in knownEnvironmentVariable.Value)
                {
                    envVariableGetter.GetVariableValue(variable).Returns("");
                }
            }

            LogExtensions.environmentVariableReader = envVariableGetter;

            LogExtensions.EnableServiceMessages(Log);

            Assert.IsFalse(LogExtensions.IsKnownEnvironment());
        }

    }
}
