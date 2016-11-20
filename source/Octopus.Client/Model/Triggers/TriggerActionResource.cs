namespace Octopus.Client.Model.Triggers
{
    public enum TriggerActionType
    {
        AutoDeploy
    }

    public abstract class TriggerActionResource
    {
        public abstract TriggerActionType ActionType { get; }
    }
}