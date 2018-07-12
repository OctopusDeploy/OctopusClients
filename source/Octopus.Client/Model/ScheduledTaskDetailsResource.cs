using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class ScheduledTaskDetailsResource : Resource
    {
        public IList<ActivityElement> ActivityLog { get; set; }
    }
}