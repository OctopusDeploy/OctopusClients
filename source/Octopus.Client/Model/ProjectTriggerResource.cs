using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;
using Octopus.Client.Model.Triggers;

namespace Octopus.Client.Model
{
    public class ProjectTriggerResource : Resource, INamedResource, IHaveSpaceResource
    {
        [Trim]
        [Writeable]
        public string Name { get; set; }

        [Writeable]
        public string ProjectId { get; set; }

        [Writeable]
        public bool IsDisabled { get; set; }


        [Writeable]
        public TriggerFilterResource Filter { get; set; }

        [Writeable]
        public TriggerActionResource Action { get; set; }

        public string SpaceId { get; set; }
    }
}
