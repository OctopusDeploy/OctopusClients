using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.ScheduledTriggers
{
    public enum ScheduledTriggerActionType
    {
        PromoteLatestRelease,
        DeployLatestRelease,
        DeployNewRelease
    }

    public abstract class ScheduledTriggerActionResource : Resource
    {
        protected ScheduledTriggerActionResource()
        {
            TenantTags = new ReferenceCollection();
            TenantIds = new ReferenceCollection();
        }

        [Writeable]
        public int ChannelId { get; set; }

        [Writeable]
        public ReferenceCollection TenantIds { get; set; }

        [Writeable]
        public ReferenceCollection TenantTags { get; set; }

        public abstract ScheduledTriggerActionType ActionType { get; }
    }
}
