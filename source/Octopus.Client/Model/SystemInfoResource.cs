using System;

namespace Octopus.Client.Model
{
    public class SystemInfoResource : Resource
    {
        public string Version { get; set; }
        public string OSVersion { get; set; }
        public long WorkingSetBytes { get; set; }
        public string ClrVersion { get; set; }
        public int ThreadCount { get; set; }
        public TimeSpan Uptime { get; set; }
    }
}