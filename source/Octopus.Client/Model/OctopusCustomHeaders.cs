using System;
using Octopus.Client.Extensions;

namespace Octopus.Client.Model
{
    internal class OctopusCustomHeaders
    {
        internal static readonly string EnvVar_TeamCity = "TEAMCITY_VERSION";
        internal static readonly string EnvVar_Bamboo = "bamboo_agentId";
        internal static readonly string EnvVar_AzureDevOps = "TF_BUILD";

        internal OctopusCustomHeaders(string buildEnvironmentContext = null)
        {
            BuildEnvironment = DetermineBuildEnvironment();
            var buildEnvironmentContextString = BuildEnvironment.ToString();
            if (!string.IsNullOrWhiteSpace(buildEnvironmentContext))
            {
                buildEnvironmentContextString += $"/{buildEnvironmentContext}";
            }

            var version = typeof(OctopusCustomHeaders).GetSemanticVersion();

            UserAgent = $"{ApiConstants.OctopusUserAgentProductName}/{version.ToNormalizedString()} {buildEnvironmentContextString}";
        }

        internal string UserAgent { get; }
        internal BuildEnvironment BuildEnvironment { get; }

        static BuildEnvironment DetermineBuildEnvironment()
        {
            var buildEnvironment = BuildEnvironment.Unspecified;

            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(EnvVar_TeamCity)))
            {
                buildEnvironment = BuildEnvironment.TeamCity;
            } else if (Environment.GetEnvironmentVariable(EnvVar_AzureDevOps) == "True")
            {
                buildEnvironment = BuildEnvironment.AzureDevOps;
            } else if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(EnvVar_Bamboo)))
            {
                buildEnvironment = BuildEnvironment.Bamboo;
            }

            return buildEnvironment;
        }
    }
}