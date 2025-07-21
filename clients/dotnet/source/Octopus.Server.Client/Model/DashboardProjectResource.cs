using System;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class DashboardProjectResource : Resource
    {
        public string Name { get; set; }
        public bool IsDisabled { get; set; }
        public string Slug { get; set; }
        public string ProjectGroupId { get; set; }
        public ReferenceCollection EnvironmentIds { get; set; }

        [JsonIgnore]
        [Obsolete("Use " + nameof(TenantedDeploymentMode) + " instead. This property wasn't populated correctly anyway.")]
        public TenantedDeploymentMode TenantDeploymentMode{get;set;}
        
        public TenantedDeploymentMode TenantedDeploymentMode { get; set; }
        public bool CanPerformUntenantedDeployment { get; set; }

        public DashboardProjectResource Copy()
        {
            var copy = (DashboardProjectResource)this.MemberwiseClone();
            copy.EnvironmentIds = new ReferenceCollection(this.EnvironmentIds);
            return copy;
        }
    }
}