namespace Octopus.Client.Model.Triggers
{
    public enum TriggerFilterType
    {
        MachineFilter
    }

    public abstract class TriggerFilterResource
    {
        public abstract TriggerFilterType FilterType { get; }
    }
}