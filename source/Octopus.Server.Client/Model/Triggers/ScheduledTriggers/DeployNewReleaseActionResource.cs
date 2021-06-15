using Octopus.Client.Extensibility.Attributes;
using Octopus.Client.Model.VersionControl;

namespace Octopus.Client.Model.Triggers.ScheduledTriggers
{
    public class DeployNewReleaseActionResource : ScopedDeploymentActionResource
    {
        public override TriggerActionType ActionType => TriggerActionType.DeployNewRelease;

        [Writeable]
        public string Variables { get; set; }

        [Writeable]
        public string EnvironmentId { get; set; }

        public VersionControlReferenceResource VersionControlReference { get; set; }
    }
}
