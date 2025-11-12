using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.DeploymentFreezes
{
    public abstract class RecurringSchedule
    {
        protected RecurringSchedule()
        {
        }

        public abstract RecurringScheduleType Type { get; }

        [Writeable]
        public int Unit { get; set; }

        [Writeable]
        public RecurringScheduleEndType EndType { get; set; } = RecurringScheduleEndType.Never;

        [Writeable]
        public int UserUtcOffsetInMinutes { get; set; }

        [Writeable]
        public DateTimeOffset? EndOnDate { get; set; }

        [Writeable]
        public int? EndAfterOccurrences { get; set; }

        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
    }
}
