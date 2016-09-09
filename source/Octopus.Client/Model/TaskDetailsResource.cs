using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class TaskDetailsResource : Resource
    {
        public TaskResource Task { get; set; }
        public IList<ActivityElement> ActivityLogs { get; set; }
        public TaskProgress Progress { get; set; }
    }
}