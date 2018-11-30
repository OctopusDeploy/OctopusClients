using System;
using System.Collections.Generic;
using Octopus.Client.Extensions;

namespace Octopus.Client.Model
{
    internal class OctopusCustomHeaders
    {
        internal static readonly Dictionary<string, BuildEnvironment> buildServerEnvironmentVariableMappings = new Dictionary<string,BuildEnvironment>
        {
            {"TEAMCITY_VERSION", BuildEnvironment.TeamCity},
            {"TF_BUILD", BuildEnvironment.AzureDevOps},
            {"bamboo_agentId", BuildEnvironment.Bamboo},
            {"APPVEYOR", BuildEnvironment.AppVeyor},
            {"BITBUCKET_BUILD_NUMBER", BuildEnvironment.BitBucket},
            {"JENKINS_URL", BuildEnvironment.Jenkins},
            {"CIRCLECI", BuildEnvironment.CircleCI},
            {"GITLAB_CI", BuildEnvironment.GitLabCI},
            {"TRAVIS", BuildEnvironment.Travis},
            {"GO_PIPELINE_LABEL", BuildEnvironment.GoCD},
            {"BITRISE_IO", BuildEnvironment.BitRise},
            {"BUDDY_WORKSPACE_ID", BuildEnvironment.Buddy},
            {"BUILDKITE", BuildEnvironment.BuildKite},
            {"CIRRUS_CI", BuildEnvironment.CirrusCI},
            {"CODEBUILD_BUILD_ARN", BuildEnvironment.AWSCodeBuild},
            {"CI_NAME", BuildEnvironment.Codeship},
            {"DRONE", BuildEnvironment.Drone},
            {"DSARI", BuildEnvironment.Dsari},
            {"HUDSON_URL", BuildEnvironment.Hudson},
            {"MAGNUM", BuildEnvironment.MagnumCI},
            {"SAILCI", BuildEnvironment.SailCI},
            {"SEMAPHORE", BuildEnvironment.Semaphore},
            {"SHIPPABLE", BuildEnvironment.Shippable},
            {"TDDIUM", BuildEnvironment.SolanoCI},
            {"STRIDER", BuildEnvironment.StriderCD},
        };

        internal OctopusCustomHeaders(string requestingTool = null)
        {
            BuildEnvironment = DetermineBuildEnvironment();
            var automationContext = BuildEnvironment.ToString();
            if (!string.IsNullOrWhiteSpace(requestingTool))
            {
                automationContext += $"/{requestingTool}";
            }

            var version = typeof(OctopusCustomHeaders).GetSemanticVersion();

            UserAgent = $"{ApiConstants.OctopusUserAgentProductName}/{version.ToNormalizedString()} {automationContext}";
        }

        internal string UserAgent { get; }
        internal BuildEnvironment BuildEnvironment { get; }

        // NOTE: this has been implemented to abstract the Environment calls so the tests can mock these calls when being
        // run on a build agent. This isn't done through DI because it we don't want it to bleed out through the OctopusClient ctors
        // BEWARE: this will likely cause issues if the tests get run in parallel
        internal static Func<string, string> GetEnvironmentVariable = Environment.GetEnvironmentVariable;

        static BuildEnvironment DetermineBuildEnvironment()
        {
            foreach (var buildServerEnvironmentVariableMapping in buildServerEnvironmentVariableMappings)
            {
                if (!string.IsNullOrWhiteSpace(GetEnvironmentVariable(buildServerEnvironmentVariableMapping.Key)))
                {
                    return buildServerEnvironmentVariableMapping.Value;
                }
            }

            return BuildEnvironment.Unspecified;
        }
    }
}