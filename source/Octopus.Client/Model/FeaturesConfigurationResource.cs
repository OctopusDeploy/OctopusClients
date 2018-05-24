using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class FeaturesConfigurationResource : Resource
    {
        [Writeable]
        public bool IsMultiTenancyEnabled { get; set; }

        [Writeable]
        public bool IsBuiltinWorkerEnabled { get; set; }

        [Writeable]
        public bool IsCommunityActionTemplatesEnabled { get; set; }

        [Writeable]
        public bool IsBuiltInRepoSyncEnabled { get; set; }
    }
}
