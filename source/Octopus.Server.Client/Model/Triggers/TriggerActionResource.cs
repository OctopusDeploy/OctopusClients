namespace Octopus.Client.Model.Triggers
{
    public enum TriggerActionType
    {
        AutoDeploy,
        DeployLatestRelease,
        DeployNewRelease,
        RunRunbook,
        CreateRelease,
        DeployLatestReleaseToEnvironment
    }

    public abstract class TriggerActionResource : Resource
    {
        public abstract TriggerActionType ActionType { get; }
    }
}