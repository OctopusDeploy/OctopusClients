using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Util
{
    public static class FeatureDetectionExtensions
    {
        public static bool SupportsChannels(this IOctopusRepository repository)
        {
            return repository?.Client?.RootDocument.SupportsChannels() == true;
        }

        public static bool SupportsChannels(this RootResource source)
        {
            return source?.HasLink("Channels") == true;
        }
    }
}