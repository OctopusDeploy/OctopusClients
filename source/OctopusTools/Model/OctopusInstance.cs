using System;

namespace OctopusTools.Model
{
    public class OctopusInstance : Resource
    {
        public string Application { get; set; }
        public string Version { get; set; }
        public string OperatingSystem { get; set; }
    }
}