using System;

namespace Octopus.Client.Model.DeploymentFreezes
{
    public class RecurringSchedule
    {
        
        public string Type { get; set; }
        public int Unit { get; set; }
        public string EndType { get; set; } = "Never";
        public int UserUtcOffsetInMinutes { get; set; }
        public DateTimeOffset? EndOnDate { get; set; }
        public int? EndAfterOccurrences { get; set; }

        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
    }
}
