using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers.ScheduledTriggers
{
    public enum TriggerScheduleType
    {
        Daily,
        DaysPerWeek,
        DaysPerMonth,
        CronExpression
    }

    public abstract class ScheduledTriggerFilterResource : TriggerFilterResource
    {
        [Writeable]
        public string Timezone { get; set; }
    }
}
