using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Util
{
    public static class FeatureDetectionExtensions
    {
        public static bool SupportsChannels(this IOctopusAsyncRepository repository)
        {
            var hasChannelLink = repository?.HasLink("Channels") == true;
            if (!hasChannelLink)
            {
                // When default space is off and SpaceId is not provided, we check if it is in post space world, as channels are always available in spaces
                return repository?.HasLink("SpaceHome") == true;
            }

            return true;
        }
        
        public static bool SupportsTenants(this IOctopusAsyncRepository repository)
        {
            var hasTenantLink = repository?.HasLink("Tenants") == true;
            if (!hasTenantLink)
            {
                // When default space is off and SpaceId is not provided, we check if it is in post space world, as tenants are always available in spaces
                return repository?.HasLink("SpaceHome") == true;
            }

            return true;
        }

        public static bool UsePostForChannelVersionRuleTest(this RootResource source)
        {
            // Assume octo.exe 3.4 should use the OctopusServer 3.4 POST, otherwise if we're certain this is an older Octopus Server use the GET method
            SemanticVersion octopusServerVersion;
            return source == null || !SemanticVersion.TryParse(source.Version, out octopusServerVersion) || octopusServerVersion >= new SemanticVersion("3.4");
        }
    }
}