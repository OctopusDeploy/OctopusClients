using System.Linq;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class MachineResource : MachineBasedResource
    {
        public MachineResource()
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
                // to preexisting behaviour
                return TenantIds.Any() || TenantTags.Any()
                    ? TenantedDeploymentMode.Tenanted
                    : TenantedDeploymentMode.Untenanted;
            }
        }

        [Writeable]
        public ReferenceCollection TenantIds { get; set; }

        [Writeable]
        public ReferenceCollection TenantTags { get; set; }

        public MachineResource AddOrUpdateEnvironments(params EnvironmentResource[] environments)
        {
            foreach (var environment in environments)
            {
                EnvironmentIds.Add(environment.Id);
            }
            return this;
        }

        public MachineResource RemoveEnvironment(EnvironmentResource environment)
        {
            EnvironmentIds.Remove(environment.Id);
            return this;
        }

        public MachineResource ClearEnvironments()
        {
            EnvironmentIds.Clear();
            return this;
        }

        public MachineResource AddOrUpdateRoles(params string[] roles)
        {
            foreach (var role in roles)
            {
                Roles.Add(role);
            }
            return this;
        }

        public MachineResource RemoveRole(string role)
        {
            Roles.Remove(role);
            return this;
        }

        public MachineResource ClearRoles()
        {
            Roles.Clear();
            return this;
        }

        public MachineResource AddOrUpdateTenants(params TenantResource[] tenants)
        {
            foreach (var tenant in tenants)
            {
                TenantIds.Add(tenant.Id);
            }
            return this;
        }

        public MachineResource RemoveTenant(TenantResource tenant)
        {
            TenantIds.Remove(tenant.Id);
            return this;
        }

        public MachineResource ClearTenants()
        {
            TenantIds.Clear();
            return this;
        }

        public MachineResource AddOrUpdateTenantTags(params TagResource[] tenantTags)
        {
            foreach (var tenantTag in tenantTags)
            {
                TenantTags.Add(tenantTag.CanonicalTagName);
            }
            return this;
        }

        public MachineResource RemoveTenantTag(TagResource tenantTag)
        {
            TenantTags.Remove(tenantTag.CanonicalTagName);
            return this;
        }

        public MachineResource ClearTenantTags()
        {
            TenantTags.Clear();
            return this;
        }
    }
}