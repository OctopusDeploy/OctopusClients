using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers.ScheduledTriggers
{
    public enum ScheduledTriggerFilterRunType
    {
        ScheduledTime,
        Continuously
    }

    public class DailyScheduledTriggerFilterResource : ScheduledTriggerFilterResource
    {
        public override TriggerFilterType FilterType => TriggerFilterType.DailySchedule;

        [Writeable]
        public DateTime? StartTime { get; set; }
        
        [Writeable]
        public DateTime? EndTime { get; set; }
        
        [Writeable]
        public ScheduledTriggerFilterRunType RunType { get; set; }

        [Writeable]
        public DailyScheduledTriggerInterval Interval { get; set; }

        [Writeable]
        public short? HourInterval { get; set; }

        [Writeable]
        public short? MinuteInterval { get; set; }
    }
}
