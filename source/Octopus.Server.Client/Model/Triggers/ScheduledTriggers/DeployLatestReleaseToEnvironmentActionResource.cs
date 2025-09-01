using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers.ScheduledTriggers
{
    public class DeployLatestReleaseToEnvironmentActionResource : ScopedDeploymentActionResource
    {
        public override TriggerActionType ActionType => TriggerActionType.DeployLatestReleaseToEnvironment;

        [Writeable]
        public string Variables { get; set; }

        [Writeable]
        public string EnvironmentId { get; set; }

        [Writeable]
        public bool ShouldRedeployWhenReleaseIsCurrent { get; set; } = true;
    }
}