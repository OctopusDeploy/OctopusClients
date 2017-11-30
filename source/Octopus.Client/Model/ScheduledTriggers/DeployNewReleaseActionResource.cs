namespace Octopus.Client.Model.ScheduledTriggers
{
    public class DeployNewReleaseActionResource : ScheduledTriggerActionResource
    {
        public override ScheduledTriggerActionType ActionType => ScheduledTriggerActionType.DeployNewRelease;

        [Writeable]
        public int EnvironmentId { get; set; }
    }
}
