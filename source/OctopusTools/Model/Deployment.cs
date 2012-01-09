using System;

namespace OctopusTools.Model
{
    public class Deployment : Resource
    {
        public int Id { get; set; }
        public int ReleaseId { get; set; }
        public int EnvironmentId { get; set; }
        public bool ForceRedeployment { get; set; }
        public string Name { get; set; }
    }
}