using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Extensions
{
    public static class DeploymentTargetResourceExtensions
    {
        public static T AddOrUpdateEnvironments<T>(this T deploymentTarget, params EnvironmentResource[] environments)
            where T : DeploymentTargetResource
        {
            foreach (var environment in environments)
            {
                deploymentTarget.EnvironmentIds.Add(environment.Id);
            }
            return deploymentTarget;
        }

        public static T RemoveEnvironment<T>(this T deploymentTarget, EnvironmentResource environment)
            where T : DeploymentTargetResource
        {
            deploymentTarget.EnvironmentIds.Remove(environment.Id);
            return deploymentTarget;
        }

        public static T ClearEnvironments<T>(this T deploymentTarget)
            where T : DeploymentTargetResource
        {
            deploymentTarget.EnvironmentIds.Clear();
            return deploymentTarget;
        }

        public static T AddOrUpdateRoles<T>(this T deploymentTarget, params string[] roles)
            where T : DeploymentTargetResource
        {
            foreach (var role in roles)
            {
                deploymentTarget.Roles.Add(role);
            }
            return deploymentTarget;
        }

        public static T RemoveRole<T>(this T deploymentTarget, string role)
            where T : DeploymentTargetResource
        {
            deploymentTarget.Roles.Remove(role);
            return deploymentTarget;
        }

        public static T ClearRoles<T>(this T deploymentTarget)
            where T : DeploymentTargetResource
        {
            deploymentTarget.Roles.Clear();
            return deploymentTarget;
        }

        public static T AddOrUpdateTenants<T>(this T deploymentTarget, params TenantResource[] tenants)
            where T : DeploymentTargetResource
        {
            foreach (var tenant in tenants)
            {
                deploymentTarget.TenantIds.Add(tenant.Id);
            }
            return deploymentTarget;
        }

        public static T RemoveTenant<T>(this T deploymentTarget, TenantResource tenant)
            where T : DeploymentTargetResource
        {
            deploymentTarget.TenantIds.Remove(tenant.Id);
            return deploymentTarget;
        }

        public static T ClearTenants<T>(this T deploymentTarget)
            where T : DeploymentTargetResource
        {
            deploymentTarget.TenantIds.Clear();
            return deploymentTarget;
        }

        public static T AddOrUpdateTenantTags<T>(this T deploymentTarget, params TagResource[] tenantTags)
            where T : DeploymentTargetResource
        {
            foreach (var tenantTag in tenantTags)
            {
                deploymentTarget.TenantTags.Add(tenantTag.CanonicalTagName);
            }
            return deploymentTarget;
        }

        public static T RemoveTenantTag<T>(this T deploymentTarget, TagResource tenantTag)
            where T : DeploymentTargetResource
        {
            deploymentTarget.TenantTags.Remove(tenantTag.CanonicalTagName);
            return deploymentTarget;
        }

        public static T ClearTenantTags<T>(this T deploymentTarget)
            where T : DeploymentTargetResource
        {
            deploymentTarget.TenantTags.Clear();
            return deploymentTarget;
        }
    }
}
