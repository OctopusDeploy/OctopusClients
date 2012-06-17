using System;

namespace OctopusTools.Model
{
    public class DeploymentEnvironment : Resource
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}