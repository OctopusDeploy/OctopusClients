using System;

namespace Octopus.Client.Model
{
    internal static class OctopusCustomHeaders
    {
        public static readonly string EnvVar_TeamCity = "TEAMCITY_VERSION";
        public static readonly string EnvVar_Bamboo = "bamboo_agentId";
        public static readonly string EnvVar_AzureDevOps = "TF_BUILD";
        public static readonly string[] BuildServerEnvVars = new[] {EnvVar_TeamCity, EnvVar_Bamboo, EnvVar_AzureDevOps};

        public static string UserAgent(SemanticVersion version)
        {
            return $"{ApiConstants.OctopusUserAgentProductName}/{version.ToNormalizedString()} {DetermineBuildServer().ToString()}";
        }

        public static BuildServer DetermineBuildServer()
        {
            var buildServer = BuildServer.Unspecified;

            if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(EnvVar_TeamCity)))
            {
                buildServer = BuildServer.TeamCity;
            } else if (Environment.GetEnvironmentVariable(EnvVar_AzureDevOps) == "True")
            {
                buildServer = BuildServer.AzureDevOps;
            } else if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(EnvVar_Bamboo)))
            {
                buildServer = BuildServer.Bamboo;
            }

            return buildServer;
        }
    }
}