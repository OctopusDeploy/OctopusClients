using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers.ScheduledTriggers
{
    public abstract class ScheduledTriggerFilterResource : TriggerFilterResource
    {
        [Writeable]
        public string Timezone { get; set; }
    }
}
