using System;

namespace Octopus.Client.Model.ScheduledTriggers
{
    public class DailyTriggerScheduleResource : TriggerScheduleResource
    {
        public override TriggerScheduleType ScheduleType => TriggerScheduleType.Daily;

        public DateTime StartTime { get; set; }

        public DailyTriggerScheduleInterval Interval { get; set; }

        public short? HourlInterval { get; set; }

        public short? MinuteInterval { get; set; }
    }
}
