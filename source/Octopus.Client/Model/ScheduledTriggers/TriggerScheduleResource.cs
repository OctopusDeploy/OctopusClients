namespace Octopus.Client.Model.ScheduledTriggers
{
    public enum TriggerScheduleType
    {
        Daily,
        DaysPerWeek,
        DaysPerMonth,
        CronExpression
    }

    public abstract class TriggerScheduleResource : Resource
    {
        public abstract TriggerScheduleType ScheduleType { get; }

        public string Timezone { get; set; }
    }
}
