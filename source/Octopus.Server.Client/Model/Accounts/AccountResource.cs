using System.Diagnostics;
using System.Linq;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Accounts
{
    [DebuggerDisplay("Name = {Name}")]
    public abstract class AccountResource : Resource, INamedResource, IHaveSpaceResource
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
        TenantedDeploymentMode? tenantedDeploymentParticipation;

        [Writeable]
        public TenantedDeploymentMode TenantedDeploymentParticipation
        {
            set => tenantedDeploymentParticipation = value;
            
            get
            {
                if (tenantedDeploymentParticipation.HasValue)
                    return tenantedDeploymentParticipation.Value;

                // Responses from server versions before TenantedDeploymentParticipation was implemented will default
                // to pre-existing behaviour 
                return TenantIds.Any() || TenantTags.Any()
                    ? TenantedDeploymentMode.Tenanted 
                    : TenantedDeploymentMode.Untenanted;
            }
        }

        [Writeable]
        public ReferenceCollection TenantIds { get; set; }

        [Writeable]
        public ReferenceCollection TenantTags { get; set; }

        [WriteableOnCreate]
        public abstract AccountType AccountType { get; }

        public string SpaceId { get; set; }
    }
}