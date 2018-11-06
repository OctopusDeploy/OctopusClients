using System;

namespace Octopus.Client.Model.Triggers
{
    public enum TriggerFilterType
    {
        [Obsolete("MachineFilter is obsolete, please use DeploymentTargetFilter instead")]
        MachineFilter,
        DeploymentTargetFilter,
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