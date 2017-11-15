using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class EnvironmentSummaryResource
    {
        public EnvironmentResource Environment { get; set; }
        public int TotalMachines { get; set; }
        public int TotalDisabledMachines { get; set; }
        public Dictionary<string, int> MachineHealthStatusSummaries { get; set; }
        public Dictionary<string, int> MachineEndpointSummaries { get; set; }
        public Dictionary<string, int> MachineTenantSummaries { get; set; }
        public Dictionary<string, int> MachineTenantTagSummaries { get; set; }
        public bool TentacleUpgradesRequired { get; set; }
        public string[] MachineIdsForCalamariUpgrade { get; set; }
    }
}
