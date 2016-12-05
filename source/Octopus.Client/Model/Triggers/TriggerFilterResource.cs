namespace Octopus.Client.Model.Triggers
{
    public enum TriggerFilterType
    {
        MachineFilter
    }

    public abstract class TriggerFilterResource : Resource
    {
        public abstract TriggerFilterType FilterType { get; }
    }
}