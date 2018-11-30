using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Model;

namespace Octopus.Client.AutomationEnvironments
{
    internal class AutomationEnvironmentProvider : IAutomationEnvironmentProvider
    {
        internal static readonly Dictionary<AutomationEnvironment, string[]> KnownEnvironmentVariables = new Dictionary<AutomationEnvironment, string[]>
        {
            { AutomationEnvironment.Octopus, new []{"AgentProgramDirectoryPath"}},
            { AutomationEnvironment.TeamCity, new [] {"TEAMCITY_VERSION"}},
            { AutomationEnvironment.AzureDevOps, new [] {"TF_BUILD", "BUILD_BUILDID", "AGENT_WORKFOLDER"}},
            { AutomationEnvironment.Bamboo, new [] {"bamboo_agentId"}},
            { AutomationEnvironment.AppVeyor, new [] {"APPVEYOR"}},
            { AutomationEnvironment.BitBucket, new [] {"BITBUCKET_BUILD_NUMBER"}},
            { AutomationEnvironment.Jenkins, new [] {"JENKINS_URL"}},
            { AutomationEnvironment.CircleCI, new [] {"CIRCLECI"}},
            { AutomationEnvironment.GitLabCI, new [] {"GITLAB_CI"}},
            { AutomationEnvironment.Travis, new [] {"TRAVIS"}},
            { AutomationEnvironment.GoCD, new [] {"GO_PIPELINE_LABEL"}},
            { AutomationEnvironment.BitRise, new [] {"BITRISE_IO"}},
            { AutomationEnvironment.Buddy, new [] {"BUDDY_WORKSPACE_ID"}},
            { AutomationEnvironment.BuildKite, new [] {"BUILDKITE"}},
            { AutomationEnvironment.CirrusCI, new [] {"CIRRUS_CI"}},
            { AutomationEnvironment.AWSCodeBuild, new [] {"CODEBUILD_BUILD_ARN"}},
            { AutomationEnvironment.Codeship, new [] {"CI_NAME"}},
            { AutomationEnvironment.Drone, new [] {"DRONE"}},
            { AutomationEnvironment.Dsari, new [] {"DSARI"}},
            { AutomationEnvironment.Hudson, new [] {"HUDSON_URL"}},
            { AutomationEnvironment.MagnumCI, new [] {"MAGNUM"}},
            { AutomationEnvironment.SailCI, new [] {"SAILCI"}},
            { AutomationEnvironment.Semaphore, new [] {"SEMAPHORE"}},
            { AutomationEnvironment.Shippable, new [] {"SHIPPABLE"}},
            { AutomationEnvironment.SolanoCI, new [] {"TDDIUM"}},
            { AutomationEnvironment.StriderCD, new [] {"STRIDER"}}
        };

        internal static IEnvironmentVariableReader environmentVariableReader = new EnvironmentVariableReader();

        static bool EnvironmentVariableHasValue(string variableName)
        {
            return !string.IsNullOrEmpty(environmentVariableReader.GetVariableValue(variableName));
        }

        public AutomationEnvironment DetermineAutomationEnvironment()
        {
            return KnownEnvironmentVariables.Where(kev => kev.Value.Any(EnvironmentVariableHasValue)).Select(x => x.Key).Distinct().FirstOrDefault();
        }
    }

    internal interface IAutomationEnvironmentProvider
    {
        AutomationEnvironment DetermineAutomationEnvironment();
    }
}