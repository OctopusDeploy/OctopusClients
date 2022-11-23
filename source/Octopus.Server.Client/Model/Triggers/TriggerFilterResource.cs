using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers
{
    public enum TriggerFilterType
    {
        MachineFilter,
        DailySchedule,
        OnceDailySchedule,
        ContinuousDailySchedule,
        DaysPerMonthSchedule,
        DaysPerWeekSchedule,
        CronExpressionSchedule
    }

    public abstract class TriggerFilterResource : Resource
    {
        public abstract TriggerFilterType FilterType { get; }
    }
}