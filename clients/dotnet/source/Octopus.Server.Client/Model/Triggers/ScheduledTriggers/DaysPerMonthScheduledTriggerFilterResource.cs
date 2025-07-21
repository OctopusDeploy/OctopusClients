﻿using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Triggers.ScheduledTriggers
{
    public enum MonthlyScheduleType
    {
        DateOfMonth,
        DayOfMonth
    }

    public enum DayOfWeek
    {
        Sunday = 0,
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6
    }

    public class DaysPerMonthScheduledTriggerFilterResource : ScheduledTriggerFilterResource
    {
        public override TriggerFilterType FilterType => TriggerFilterType.DaysPerMonthSchedule;

        [Writeable]
        public DateTime StartTime { get; set; }
        
        [Writeable]
        public MonthlyScheduleType MonthlyScheduleType { get; set; }

        [Writeable]
        public string DateOfMonth { get; set; }

        [Writeable]
        public string DayNumberOfMonth { get; set; }

        [Writeable]
        public DayOfWeek? DayOfWeek { get; set; }
    }
}
