namespace Octopus.Client.Model.Triggers
{
    public enum TriggerFilterType
    {
        MachineFilter,
        DailySchedule,
        DaysPerWeekSchedule,
        DaysPerMonthSchedule,
        CronExpressionSchedule
    }

    public abstract class TriggerFilterResource : Resource
    {
        public abstract TriggerFilterType FilterType { get; }
    }
}