using System;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class DashboardConfigurationResource : Resource, IHaveSpaceResource
    {
        public DashboardConfigurationResource()
        {
            IncludedProjectGroupIds = new ReferenceCollection();
            IncludedProjectIds = new ReferenceCollection();
            IncludedProjectTags = new ReferenceCollection();
            IncludedEnvironmentIds = new ReferenceCollection();
            IncludedEnvironmentTags = new ReferenceCollection();
            IncludedTenantIds = new ReferenceCollection();
            IncludedTenantTags = new ReferenceCollection();
        }

        [Writeable]
        public ReferenceCollection IncludedProjectGroupIds { get; set; }

        [Writeable]
        public ReferenceCollection IncludedProjectIds { get; set; }

        [Writeable]
        public ReferenceCollection IncludedProjectTags { get; set; }

        [Writeable]
        public ReferenceCollection IncludedEnvironmentIds { get; set; }

        [Writeable]
        public ReferenceCollection IncludedEnvironmentTags { get; set; }

        [Writeable]
        public ReferenceCollection IncludedTenantIds { get; set; }

        [Writeable]
        public int? ProjectLimit { get; set; }

        [Writeable]
        public ReferenceCollection IncludedTenantTags { get; set; }

        public string SpaceId { get; set; }
    }
}