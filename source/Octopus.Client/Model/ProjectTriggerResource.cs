using Octopus.Client.Model.Triggers;

namespace Octopus.Client.Model
{
    public class ProjectTriggerResource : Resource, INamedResource
    {
        [Writeable]
        public string Name { get; set; }

        [Writeable]
        public string ProjectId { get; set; }

        [Writeable]
        public ProjectTriggerType Type { get; set; }

        [Writeable]
        public TriggerFilterResource Filter { get; set; }

        [Writeable]
        public TriggerActionResource Action { get; set; }
    }
}
