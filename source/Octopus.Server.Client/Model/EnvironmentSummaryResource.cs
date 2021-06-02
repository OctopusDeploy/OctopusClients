using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class EnvironmentSummaryResource : SummaryResource
    {
        public EnvironmentResource Environment { get; set; }
        public Dictionary<string, int> MachineTenantSummaries { get; set; }
        public Dictionary<string, int> MachineTenantTagSummaries { get; set; }
    }
}
