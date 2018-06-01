using System.Linq;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class DeploymentTargetResource : MachineBasedResource
    {
        public DeploymentTargetResource()
        {
            EnvironmentIds = new ReferenceCollection();
            Roles = new ReferenceCollection();
            TenantTags = new ReferenceCollection();
            TenantIds = new ReferenceCollection();
        }

        [Writeable]
        public ReferenceCollection EnvironmentIds { get; set; }

        [Writeable]
        public ReferenceCollection Roles { get; set; }

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
    }
}