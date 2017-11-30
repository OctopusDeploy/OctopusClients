using System;

namespace Octopus.Client.Model.ScheduledTriggers
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

    public class DaysPerMonthTriggerScheduleResource : TriggerScheduleResource
    {
        public override TriggerScheduleType ScheduleType => TriggerScheduleType.DaysPerMonth;

        public DateTime StartTime { get; set; }

        public MonthlyScheduleType MonthlyScheduleType { get; set; }

        public short? DateOfMonth { get; set; }

        public short? DayNumberOfMonth { get; set; }

        public DayOfWeek DayOfWeek { get; set; }
    }
}
