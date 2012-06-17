using System;

namespace OctopusTools.Model
{
    public class RootDocument : Resource
    {
        public string Application { get; set; }
        public string ApiVersion { get; set; }
        public string Version { get; set; }
        public string OperatingSystem { get; set; }
    }
}