using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers
{
    public enum TriggerFilterType
    {
        MachineFilter,
        OnceDailySchedule,
        ContinuousDailySchedule,
        ContinuousDailyForDaysPerWeekSchedule,
        OnceDailyForDaysPerWeekSchedule,
        DaysPerMonthSchedule,
        CronExpressionSchedule
    }

    public abstract class TriggerFilterResource : Resource
    {
        public abstract TriggerFilterType FilterType { get; }
    }
}