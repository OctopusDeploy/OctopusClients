using System;

namespace Octopus.Client.Model
{
    public class OctopusServerNodeResource : Resource, INamedResource
    {
        public string Name { get; set; }
        public string LastSeen { get; set; }
        public string Rank { get; set; }
        public bool IsOffline { get; set; }
        [Writeable]
        public int MaxConcurrentTasks { get; set; }
        [Writeable]
        public bool IsInMaintenanceMode { get; set; }
    }
}
