using System;

namespace Octopus.Client.Model
{
    public class MaintenanceConfigurationResource : Resource
    {
        [Writeable]
        public bool IsInMaintenanceMode { get; set; }
    }
}