using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.ScheduledTriggers
{
    public class PromoteLatestReleaseActionResource : ScheduledTriggerActionResource
    {
        public override ScheduledTriggerActionType ActionType => ScheduledTriggerActionType.PromoteLatestRelease;

        [Writeable]
        public int SourceEnvironmentId { get; set; }

        [Writeable]
        public int DestinationEnvironmentId { get; set; }
    }
}
