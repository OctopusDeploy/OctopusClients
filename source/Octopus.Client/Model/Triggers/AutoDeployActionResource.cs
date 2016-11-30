namespace Octopus.Client.Model.Triggers
{
    public class AutoDeployActionResource : TriggerActionResource
    {
        public override TriggerActionType ActionType => TriggerActionType.AutoDeploy;

        [Writeable]
        public bool ShouldRedeployWhenMachineHasBeenDeployedTo { get; set; }
    }
}