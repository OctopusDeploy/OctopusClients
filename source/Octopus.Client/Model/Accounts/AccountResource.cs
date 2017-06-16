using System;
using System.Linq;
using Octopus.Client.Model.Tenants;

namespace Octopus.Client.Model.Accounts
{
    public abstract class AccountResource : Resource, INamedResource
    {
        protected AccountResource()
        {
            EnvironmentIds = new ReferenceCollection();
            TenantTags = new ReferenceCollection();
            TenantIds = new ReferenceCollection();
        }

        [Writeable]
        [Trim]
        public string Name { get; set; }

        [Writeable]
        public string Description { get; set; }

        [Writeable]
        public ReferenceCollection EnvironmentIds { get; set; }

        // Nullable backing-field is to support backwards-compatibility
        TenantedDeploymentParticipation? tenantedDeploymentParticipation;

        [Writeable]
        public TenantedDeploymentParticipation TenantedDeploymentParticipation
        {
            set => tenantedDeploymentParticipation = value;
            
            get
            {
                if (tenantedDeploymentParticipation.HasValue)
                    return tenantedDeploymentParticipation.Value;

                // Responses from server versions before TenantedDeploymentParticipation was implemented will default
                // to pre-existing behaviour 
                return TenantIds.Any() || TenantTags.Any()
                    ? TenantedDeploymentParticipation.IncludedInTenanted
                    : TenantedDeploymentParticipation.Excluded;
            }
        }

        [Writeable]
        public ReferenceCollection TenantIds { get; set; }

        [Writeable]
        public ReferenceCollection TenantTags { get; set; }

        [WriteableOnCreate]
        public abstract AccountType AccountType { get; }
    }
}