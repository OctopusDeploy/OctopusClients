using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.DeploymentFreezes
{
    public class WeeklyRecurringSchedule : RecurringSchedule
    {
        public WeeklyRecurringSchedule()
        {
            DaysOfWeek = DaysOfWeek.Monday;
        }

        public override RecurringScheduleType Type => RecurringScheduleType.Weekly;

        [Writeable]
        [Required]
        public DaysOfWeek DaysOfWeek { get; set; }
    }
}
