using Octopus.Client.Model.ScheduledTriggers;

namespace Octopus.Client.Model
{
    public class ScheduledProjectTriggerResource : Resource, INamedResource
    {
        [Trim]
        [Writeable]
        public string Name { get; set; }

        [Writeable]
        public string ProjectId { get; set; }

        [Writeable]
        public TriggerScheduleResource Schedule { get; set; }

        [Writeable]
        public ScheduledTriggerActionResource Action { get; set; }
    }
}
