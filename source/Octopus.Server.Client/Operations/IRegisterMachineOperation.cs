using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Operations
{
    /// <summary>
    /// Encapsulates the operation for registering machines.
    /// </summary>
    public interface IRegisterMachineOperation : IRegisterMachineOperationBase
    {
        /// <summary>
        /// Gets or sets the environments that this machine should be added to. These are environment names only.
        /// </summary>
        [Obsolete($"Use the {nameof(Environments)} property as it supports environment names, slugs and Ids.")]
        public string[] EnvironmentNames { get; set; }

        /// <summary>
        /// Gets or sets the environments that this machine should be added to. These can be environment names, slugs or Ids
        /// </summary>
        public string[] Environments { get; set; }

        /// <summary>
        /// Gets or sets the roles that this machine belongs to.
        /// </summary>
        string[] Roles { get; set; }

        /// <summary>
        /// Gets or sets the tenants that this machine is linked to.
        /// </summary>
        string[] Tenants { get; set; }

        /// <summary>
        /// Gets or sets the tenant tags that this machine is linked to.
        /// </summary>
        string[] TenantTags { get; set; }

        /// <summary>
        /// How the machine should participate in Tenanted Deployments.
        /// Allowed values are Untenanted, TenantedOrUntenanted or Tenanted
        /// </summary>
        TenantedDeploymentMode TenantedDeploymentParticipation { get; set; }
    }
}