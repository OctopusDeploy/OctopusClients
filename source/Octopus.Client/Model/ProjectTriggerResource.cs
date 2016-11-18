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
        public IProjectTriggerFilterResource Filter { get; set; }

        [Writeable]
        public IProjectTriggerActionResource Action { get; set; }
    }
}
