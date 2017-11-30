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
        [Writeable]
        public int ChannelId { get; set; }

        public abstract ScheduledTriggerActionType ActionType { get; }
    }
}
