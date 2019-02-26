using System;
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
    }
}
