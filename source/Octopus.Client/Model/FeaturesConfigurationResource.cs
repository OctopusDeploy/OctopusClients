using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class FeaturesConfigurationResource : Resource
    {
        [Writeable]
        public bool IsBuiltInWorkerEnabled { get; set; } = true;

        [Writeable]
        public bool IsCommunityActionTemplatesEnabled { get; set; }
    }
}
