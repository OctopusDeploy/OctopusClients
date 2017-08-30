using System;

namespace Octopus.Client.Model
{
    public class ActivityElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset? Started { get; set;}
        public DateTimeOffset? Ended { get; set;}
        public ActivityStatus Status { get; set; }
        public ActivityElement[] Children { get; set; }
        public bool ShowAtSummaryLevel { get; set; }
        public ActivityLogElement[] LogElements { get; set; }
        public int ProgressPercentage { get; set; }
        public string ProgressMessage { get; set; }
    }
}