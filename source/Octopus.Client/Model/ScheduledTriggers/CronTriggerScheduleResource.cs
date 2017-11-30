namespace Octopus.Client.Model.ScheduledTriggers
{
    public class CronTriggerScheduleResource : TriggerScheduleResource
    {
        public override TriggerScheduleType ScheduleType => TriggerScheduleType.CronExpression;

        public string CronExpression { get; set; }

    }
}
