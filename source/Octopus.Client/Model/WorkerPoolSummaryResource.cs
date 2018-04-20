using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class WorkerPoolSummaryResource : SummaryResource
    {
        public WorkerPoolResource WorkerPool { get; set; }
    }
}
