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
        public string Rank { get; set; }
        public bool IsLeader { get; set; }
        public bool IsOffline { get; set; }
        public int RunningTaskCount { get;set; }
    }
}
