using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers.ScheduledTriggers
{
    public enum ScheduledTriggerActionType
    {
        DeployLatestRelease,
        DeployNewRelease
    }

    public abstract class ScheduledTriggerActionResource : TriggerActionResource
    {
        protected ScheduledTriggerActionResource()
        {
            TenantTags = new ReferenceCollection();
            TenantIds = new ReferenceCollection();
        }

        [Writeable]
        public string ChannelId { get; set; }

        [Writeable]
        public ReferenceCollection TenantIds { get; set; }

        [Writeable]
        public ReferenceCollection TenantTags { get; set; }
    }
}
