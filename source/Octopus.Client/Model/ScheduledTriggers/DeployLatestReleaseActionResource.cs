using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.ScheduledTriggers
{
    public class DeployLatestReleaseActionResource : ScheduledTriggerActionResource
    {
        public override ScheduledTriggerActionType ActionType => ScheduledTriggerActionType.DeployLatestRelease;

        [Writeable]
        public int EnvironmentId { get; set; }
    }
}
