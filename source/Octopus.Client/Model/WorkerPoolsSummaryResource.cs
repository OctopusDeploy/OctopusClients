using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class WorkerPoolsSummaryResource : SummaryResourcesCombined
    {
        public List<WorkerPoolSummaryResource> WorkerPoolSummaries { get; set; }
    }
}
