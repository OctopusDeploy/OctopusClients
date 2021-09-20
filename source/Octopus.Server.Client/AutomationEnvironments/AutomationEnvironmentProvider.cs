using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Logging;
using Octopus.Client.Model;

namespace Octopus.Client.AutomationEnvironments
{
  public class AutomationEnvironmentProvider : IAutomationEnvironmentProvider
  {
    private static readonly ILog Logger = LogProvider.For<AutomationEnvironmentProvider>();

    internal static readonly Dictionary<AutomationEnvironment, string[]> KnownEnvironmentVariables = new Dictionary<AutomationEnvironment, string[]>
        {
            { AutomationEnvironment.Octopus, new []{"AgentProgramDirectoryPath"}},
            // https://confluence.jetbrains.com/display/TCD9/Predefined+Build+Parameters
            { AutomationEnvironment.TeamCity, new [] {"TEAMCITY_VERSION"}},
            // https://docs.microsoft.com/en-us/azure/devops/pipelines/build/variables
            { AutomationEnvironment.AzureDevOps, new [] {"TF_BUILD", "BUILD_BUILDID", "AGENT_WORKFOLDER"}},
            // https://confluence.atlassian.com/bamboo/bamboo-variables-289277087.html
            { AutomationEnvironment.Bamboo, new [] {"bamboo_agentId"}},
            { AutomationEnvironment.AppVeyor, new [] {"APPVEYOR"}},
            { AutomationEnvironment.BitBucket, new [] {"BITBUCKET_BUILD_NUMBER"}},
            { AutomationEnvironment.Jenkins, new [] {"JENKINS_URL"}},
            { AutomationEnvironment.CircleCI, new [] {"CIRCLECI"}},
            // https://docs.github.com/en/actions/reference/environment-variables#default-environment-variables
            { AutomationEnvironment.GitHubActions, new [] {"GITHUB_ACTIONS"}},
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

    public string DetermineAutomationEnvironmentWithVersion()
    {
      var environment = DetermineAutomationEnvironment();
      var envString = environment.ToString();

      if (environment == AutomationEnvironment.TeamCity)
      {
        // the TeamCity version is formatted like "2018.1.3 (Build 12345)", we just want the bit before the first space
        envString = $"{environment}/{environmentVariableReader.GetVariableValue(KnownEnvironmentVariables[environment].First()).Split(' ').First()}";
      }

      Logger.InfoFormat("Detected automation environment: {environment}", envString);

      return envString;
    }
  }

  internal interface IAutomationEnvironmentProvider
  {
    AutomationEnvironment DetermineAutomationEnvironment();
    string DetermineAutomationEnvironmentWithVersion();
  }
}