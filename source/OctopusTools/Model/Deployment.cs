using System;

namespace OctopusTools.Model
{
    public class Deployment : Resource
    {
        public string Id { get; set; }
        public string ReleaseId { get; set; }
        public string EnvironmentId { get; set; }
        public bool ForceRedeployment { get; set; }
        public bool ForcePackageDownload { get; set; }
        public string Name { get; set; }
    }
}