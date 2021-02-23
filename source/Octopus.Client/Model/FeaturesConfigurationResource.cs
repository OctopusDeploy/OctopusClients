using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class FeaturesConfigurationResource : Resource
    {
        [Writeable]
        public bool IsBuiltInWorkerEnabled { get; set; }

        [Writeable]
        public bool IsCommunityActionTemplatesEnabled { get; set; }

        [Writeable]
        public bool IsKubernetesEnabled { get; set; }

        [Writeable]
        public bool IsHelpSidebarEnabled { get; set; }

        [Writeable]
        public string HelpSidebarSupportLink { get; set; }

        [Writeable]
        public bool IsActionContainersEnabled { get; set; }

        [Writeable]
        public bool IsConfigurationAsCodeEnabled { get; set; }
    }
}
