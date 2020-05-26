using System;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class OctopusServerNodeSummaryResource : Resource
    {
        public string Name { get; set; }
        public int MaxConcurrentTasks { get; set; }
        public bool IsInMaintenanceMode { get; set; }
        public DateTimeOffset? LastSeen { get; set; }

        [Obsolete("Removed in Octopus Server 2020.3.0")]
        public LeadershipRank Rank { get; set; }

        [Obsolete("Removed in Octopus Server 2020.3.0")]
        public bool IsLeader { get; set; }

        public bool IsOffline { get; set; }
        public int RunningTaskCount { get; set; }
    }
}