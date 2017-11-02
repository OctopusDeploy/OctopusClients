using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Octopus.Cli.Commands;
using Octopus.Cli.Diagnostics;
using Octopus.Cli.Infrastructure;
using Octopus.Cli.Tests.Commands;
using Octopus.Cli.Tests.Helpers;
using Octopus.Cli.Util;

namespace Octo.Tests.Commands
{
    class ServiceMessagesTestFixture : ApiCommandFixtureBase
    {
        CreateReleaseCommand createReleaseCommand;
        IPackageVersionResolver versionResolver;
        IReleasePlanBuilder releasePlanBuilder;
        IEnvVariableGetter envVariableGetter;

        string[] buildVariables =
        {
            "TEAMCITY_VERSION", // Teamcity Variable
            "BUILD_BUILDID", // TFS/VSTS variable
            "GENT_WORKFOLDER", // TFS/VSTS variable
        };

        [Test]
        public void EnvironmentIsKnownIfBuildVariablesHaveValues()
        {
            foreach (var buildServerEnvVar in buildVariables)
            {
                envVariableGetter = Substitute.For<IEnvVariableGetter>();

                envVariableGetter.GetVariableValue(buildServerEnvVar).Returns("whatever value");

                LogExtensions.variableGetter = envVariableGetter;

                LogExtensions.EnableServiceMessages(Log);

                Assert.IsTrue(LogExtensions.IsKnownEnvironment());
            }
        }

        [Test]
        public void EnvironmentIsUnknownIfBuildVariablesDontHaveValues()
        {
            envVariableGetter = Substitute.For<IEnvVariableGetter>();

            foreach (var buildServerEnvVar in buildVariables)
            {
                envVariableGetter.GetVariableValue(buildServerEnvVar).Returns("");
            }

            LogExtensions.variableGetter = envVariableGetter;

            LogExtensions.EnableServiceMessages(Log);

            Assert.IsFalse(LogExtensions.IsKnownEnvironment());
        }

    }
}
