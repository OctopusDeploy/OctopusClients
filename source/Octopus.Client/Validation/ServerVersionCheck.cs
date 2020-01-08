using System.Linq;
using Octopus.Client.Model;
    
namespace Octopus.Client.Validation
{
    internal static class ServerVersionCheck
    {
        private static readonly string[] WhitelistOfServerVersionsSafeToIgnore = { "0.0.0-local" };

        public static bool IsOlderThanClient(string currentServerVersion, SemanticVersion minimumRequiredVersion)
        {
            if (WhitelistOfServerVersionsSafeToIgnore.Contains(currentServerVersion)) 
                return false;

            var currentVersion = Normalize(SemanticVersion.Parse(currentServerVersion));
            return currentVersion < Normalize(minimumRequiredVersion);
        }

        private static SemanticVersion Normalize(SemanticVersion version)
        {
            return new SemanticVersion(version.Major, version.Minor, version.Patch);
        }
    }
}