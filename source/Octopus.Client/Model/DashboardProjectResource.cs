using System;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class DashboardProjectResource : Resource
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public string ProjectGroupId { get; set; }
        public ReferenceCollection EnvironmentIds { get; set; }

        [JsonIgnore]
        public TenantedDeploymentMode TenantDeploymentMode { get; set; }
        public bool CanPerformUntenantedDeployment { get; set; }

        public DashboardProjectResource Copy()
        {
            var copy = (DashboardProjectResource)this.MemberwiseClone();
            copy.EnvironmentIds = new ReferenceCollection(this.EnvironmentIds);
            return copy;
        }
    }
}