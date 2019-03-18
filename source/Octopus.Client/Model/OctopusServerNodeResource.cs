using System;
using System.Collections.Generic;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class OctopusServerNodeResource : Resource, INamedResource
    {
        public string Name { get; set; }
        [Writeable]
        public int MaxConcurrentTasks { get; set; }
        [Writeable]
        public bool IsInMaintenanceMode { get; set; }
        [WriteableOnCreate]
        public int ServicesPort { get; set; }
        [WriteableOnCreate]
        public string WebSocketPrefix { get; set; }
        [WriteableOnCreate]
        public List<string> WebPortalListenPrefixes { get; set; }
        [WriteableOnCreate]
        public bool WebPortalForceSsl { get; set; }
        [WriteableOnCreate]
        public bool RequestLoggingEnabled { get; set; }
        [WriteableOnCreate]
        public bool RequestMetricLoggingEnabled { get; set; }

    }
}
