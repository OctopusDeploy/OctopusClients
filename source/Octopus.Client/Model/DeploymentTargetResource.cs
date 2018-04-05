using System.Linq;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Model
{
    public class DeploymentTargetResource : MachineResource
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

        public DeploymentTargetResource AddOrUpdateEnvironments(params EnvironmentResource[] environments)
        {
            foreach (var environment in environments)
            {
                EnvironmentIds.Add(environment.Id);
            }
            return this;
        }

        public DeploymentTargetResource RemoveEnvironment(EnvironmentResource environment)
        {
            EnvironmentIds.Remove(environment.Id);
            return this;
        }

        public DeploymentTargetResource ClearEnvironments()
        {
            EnvironmentIds.Clear();
            return this;
        }

        public DeploymentTargetResource AddOrUpdateRoles(params string[] roles)
        {
            foreach (var role in roles)
            {
                Roles.Add(role);
            }
            return this;
        }

        public DeploymentTargetResource RemoveRole(string role)
        {
            Roles.Remove(role);
            return this;
        }

        public DeploymentTargetResource ClearRoles()
        {
            Roles.Clear();
            return this;
        }

        public DeploymentTargetResource AddOrUpdateTenants(params TenantResource[] tenants)
        {
            foreach (var tenant in tenants)
            {
                TenantIds.Add(tenant.Id);
            }
            return this;
        }

        public DeploymentTargetResource RemoveTenant(TenantResource tenant)
        {
            TenantIds.Remove(tenant.Id);
            return this;
        }

        public DeploymentTargetResource ClearTenants()
        {
            TenantIds.Clear();
            return this;
        }

        public DeploymentTargetResource AddOrUpdateTenantTags(params TagResource[] tenantTags)
        {
            foreach (var tenantTag in tenantTags)
            {
                TenantTags.Add(tenantTag.CanonicalTagName);
            }
            return this;
        }

        public DeploymentTargetResource RemoveTenantTag(TagResource tenantTag)
        {
            TenantTags.Remove(tenantTag.CanonicalTagName);
            return this;
        }

        public DeploymentTargetResource ClearTenantTags()
        {
            TenantTags.Clear();
            return this;
        }
    }
}