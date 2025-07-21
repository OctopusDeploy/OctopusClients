using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers.ScheduledTriggers
{
    public class OnceDailyScheduledTriggerFilterResource : ScheduledTriggerFilterResource
    {
        public override TriggerFilterType FilterType => TriggerFilterType.OnceDailySchedule;

        [Writeable]
        public DateTime StartTime { get; set; }

        [Writeable]
        public DaysOfWeek DaysOfWeek { get; set; }
    }
}
