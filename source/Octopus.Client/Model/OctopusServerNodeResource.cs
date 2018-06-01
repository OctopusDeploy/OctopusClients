using System;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class OctopusServerNodeHeartbeatPolicy
    {
        public static TimeSpan DefaultOfflineTimeout = TimeSpan.FromMinutes(30);

        public enum HeartbeatDeletionPolicy
        {
            NeverDelete,
            DeleteAfterOfflineTimeout
        }

        public HeartbeatDeletionPolicy DeletionPolicy { get; set; } = HeartbeatDeletionPolicy.NeverDelete;
        public TimeSpan OfflineTimeout { get; set; } = DefaultOfflineTimeout;
    }

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
        [Writeable]
        public OctopusServerNodeHeartbeatPolicy HeartbeatPolicy { get; set; }
    }
}
