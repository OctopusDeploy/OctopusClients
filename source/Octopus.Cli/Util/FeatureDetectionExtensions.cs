using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Util
{
    public static class FeatureDetectionExtensions
    {
        public static bool SupportsChannels(this IOctopusAsyncRepository repository)
        {
            return repository?.Client?.HasLink("Channels") == true;
        }
        
        public static bool SupportsTenants(this IOctopusAsyncRepository repository)
        {
            return repository?.Client?.HasLink("Tenants") == true;
        }

        public static bool UsePostForChannelVersionRuleTest(this RootResource source)
        {
            // Assume octo.exe 3.4 should use the OctopusServer 3.4 POST, otherwise if we're certain this is an older Octopus Server use the GET method
            SemanticVersion octopusServerVersion;
            return source == null || !SemanticVersion.TryParse(source.Version, out octopusServerVersion) || octopusServerVersion >= new SemanticVersion("3.4");
        }
    }
}