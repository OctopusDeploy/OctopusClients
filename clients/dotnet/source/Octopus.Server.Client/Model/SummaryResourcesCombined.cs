﻿using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class SummaryResourcesCombined
    {
        public int TotalMachines { get; set; }
        public int TotalDisabledMachines { get; set; }
        public Dictionary<string, int> MachineHealthStatusSummaries { get; set; }
        public Dictionary<string, int> MachineEndpointSummaries { get; set; }
        public bool TentacleUpgradesRequired { get; set; }
        public string[] MachineIdsForCalamariUpgrade { get; set; }
        public string[] MachineIdsForTentacleUpgrade { get; set; }
    }
}
