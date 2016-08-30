using System;

namespace Octopus.Client.Model
{
    public class ServerStatusResource : Resource
    {
        public bool IsDatabaseEncrypted { get; set; }
        public AuthenticationMode AuthenticationMode { get; set; }
        public bool IsUpgradeAvailable { get; set; }
        public string MaximumAvailableVersion { get; set; }
        public string MaximumAvailableVersionCoveredByLicense { get; set; }
        public string MaintenanceExpires { get; set; }
        public bool IsInMaintenanceMode { get; set; }
    }
}