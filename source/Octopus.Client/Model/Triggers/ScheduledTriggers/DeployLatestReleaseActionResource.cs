using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers.ScheduledTriggers
{
    public class DeployLatestReleaseActionResource : ScheduledTriggerActionResource
    {
        public override TriggerActionType ActionType => TriggerActionType.DeployLatestRelease;

        [Writeable]
        public string SourceEnvironmentId { get; set; }

        [Writeable]
        public string DestinationEnvironmentId { get; set; }
    }
}
