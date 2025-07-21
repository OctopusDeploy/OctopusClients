using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class EnvironmentsSummaryResource : SummaryResourcesCombined
    {
        public Dictionary<string, int> MachineTenantSummaries { get; set; }
        public Dictionary<string, int> MachineTenantTagSummaries { get; set; }
        public List<EnvironmentSummaryResource> EnvironmentSummaries { get; set; }
    }
}
