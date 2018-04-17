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
        /// Gets or sets the environments that this machine should be added to.
        /// </summary>
        string[] EnvironmentNames { get; set; }

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
    }
}