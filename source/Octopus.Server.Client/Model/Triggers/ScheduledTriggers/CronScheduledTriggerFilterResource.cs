using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers.ScheduledTriggers
{
    public class CronScheduledTriggerFilterResource : ScheduledTriggerFilterResource
    {
        public override TriggerFilterType FilterType => TriggerFilterType.CronExpressionSchedule;

        [Writeable]
        public string CronExpression { get; set; }
    }
}
