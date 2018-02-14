using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.ScheduledTriggers
{
    public class DailyTriggerScheduleResource : TriggerScheduleResource
    {
        public override TriggerScheduleType ScheduleType => TriggerScheduleType.Daily;

        [Writeable]
        public DateTime StartTime { get; set; }

        [Writeable]
        public DailyTriggerScheduleInterval Interval { get; set; }

        [Writeable]
        public short? HourInterval { get; set; }

        [Writeable]
        public short? MinuteInterval { get; set; }
    }
}
