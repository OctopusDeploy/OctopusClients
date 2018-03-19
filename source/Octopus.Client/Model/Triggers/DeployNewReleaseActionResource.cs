using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers.ScheduledTriggers
{
    public class DeployNewReleaseActionResource : ScopedDeploymentActionResource
    {
        public override TriggerActionType ActionType => TriggerActionType.DeployNewRelease;

        [Writeable]
        public string Variables { get; set; }

        [Writeable]
        public string EnvironmentId { get; set; }
    }
}
