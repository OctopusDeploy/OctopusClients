using System.Linq;
using Octopus.Client.Model;
    
namespace Octopus.Client.Validation
{
    internal static class ServerVersionCheck
    {
        private static readonly string[] WhitelistOfServerVersionsSafeToIgnore = { "0.0.0-local" };

        public static bool IsOlderThanClient(string currentServerVersion, SemanticVersion minimumRequiredVersion)
        {
            if (WhitelistOfServerVersionsSafeToIgnore.Contains(currentServerVersion)) return false;
                
            return SemanticVersion.Parse(currentServerVersion) < minimumRequiredVersion;
        }
    }
}