using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers.ScheduledTriggers
{
    public class ContinuousDailyScheduledTriggerFilterResource : ScheduledTriggerFilterResource
    {
        public override TriggerFilterType FilterType => TriggerFilterType.ContinuousDailySchedule;

        [Writeable]
        public DateTime RunAfter { get; set; }

        [Writeable]
        public DateTime RunUntil { get; set; }

        [Writeable]
        public DailyScheduledTriggerInterval Interval { get; set; }

        [Writeable]
        public short? HourInterval { get; set; }

        [Writeable]
        public short? MinuteInterval { get; set; }

        [Writeable]
        public DaysOfWeek DaysOfWeek { get; set; }
    }
}
