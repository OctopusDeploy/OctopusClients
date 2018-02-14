using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.ScheduledTriggers
{
    public class CronTriggerScheduleResource : TriggerScheduleResource
    {
        public override TriggerScheduleType ScheduleType => TriggerScheduleType.CronExpression;

        [Writeable]
        public string CronExpression { get; set; }

    }
}
