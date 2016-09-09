using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public class MachineConnectionStatus : Resource
    {
        public string MachineId { get; set; }
        public List<ActivityLogElement> Logs { get; set; }
        public string Status { get; set; } // connection status
        public string CurrentTentacleVersion { get; set; }
        public DateTimeOffset LastChecked { get; set; }
    }
}