using System;
using Octopus.Client.Extensions;

namespace Octopus.Client.Model
{
    internal class OctopusCustomHeaders
    {
        internal static readonly string EnvVar_TeamCity = "TEAMCITY_VERSION";
        internal static readonly string EnvVar_Bamboo = "bamboo_agentId";
        internal static readonly string EnvVar_AzureDevOps = "TF_BUILD";

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

        internal static Func<string, string> GetEnvironmentVariable = Environment.GetEnvironmentVariable;

        static BuildEnvironment DetermineBuildEnvironment()
        {
            var buildEnvironment = BuildEnvironment.Unspecified;

            if (!string.IsNullOrWhiteSpace(GetEnvironmentVariable(EnvVar_TeamCity)))
            {
                buildEnvironment = BuildEnvironment.TeamCity;
            } else if (GetEnvironmentVariable(EnvVar_AzureDevOps) == "True")
            {
                buildEnvironment = BuildEnvironment.AzureDevOps;
            } else if (!string.IsNullOrWhiteSpace(GetEnvironmentVariable(EnvVar_Bamboo)))
            {
                buildEnvironment = BuildEnvironment.Bamboo;
            }

            return buildEnvironment;
        }
    }
}