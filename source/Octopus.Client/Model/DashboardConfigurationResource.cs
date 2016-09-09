using System;

namespace Octopus.Client.Model
{
    public class DashboardConfigurationResource : Resource
    {
        public DashboardConfigurationResource()
        {
            IncludedProjectIds = new ReferenceCollection();
            IncludedEnvironmentIds = new ReferenceCollection();
            IncludedTenantIds = new ReferenceCollection();
            IncludedTenantTags = new ReferenceCollection();
        }

        [Writeable]
        public ReferenceCollection IncludedProjectIds { get; set; }

        [Writeable]
        public ReferenceCollection IncludedEnvironmentIds { get; set; }

        [Writeable]
        public ReferenceCollection IncludedTenantIds { get; set; }

        [Writeable]
        public int? ProjectLimit { get; set; }

        [Writeable]
        public ReferenceCollection IncludedTenantTags { get; set; }
    }
}