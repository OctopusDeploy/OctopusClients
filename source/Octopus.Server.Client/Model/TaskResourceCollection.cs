using System.Collections.Generic;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class TaskResourceCollection : ResourceCollection<TaskResource>
    {
        public TaskResourceCollection(IEnumerable<TaskResource> items, LinkCollection links) : base(items, links)
        {
        }
        
        public Dictionary<string, int> TotalCounts { get; set; }
    }
}