using System;
using Octopus.Client.Extensibility.Attributes;

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
        
        [Writeable]
        public DateTime? RunAfter { get; set; }
        
        [Writeable]
        public DateTime? RunUntil { get; set; }
    }
}