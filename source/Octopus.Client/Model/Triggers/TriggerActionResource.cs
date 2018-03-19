namespace Octopus.Client.Model.Triggers
{
    public enum TriggerActionType
    {
        AutoDeploy,
        DeployLatestRelease,
        DeployNewRelease
    }

    public abstract class TriggerActionResource : Resource
    {
        public abstract TriggerActionType ActionType { get; }
    }
}