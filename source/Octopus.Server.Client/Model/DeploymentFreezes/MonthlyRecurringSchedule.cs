using Octopus.Client.Extensibility.Attributes;
using Octopus.Client.Model.Triggers.ScheduledTriggers;

namespace Octopus.Client.Model.DeploymentFreezes
{
    public class MonthlyRecurringSchedule : RecurringSchedule
    {
        public override RecurringScheduleType Type => RecurringScheduleType.Monthly;

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
