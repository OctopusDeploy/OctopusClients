using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Operations
{
    /// <summary>
    /// Encapsulates the operation for registering machines.
    /// </summary>
    public interface IRegisterMachineOperation
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

        /// <summary>
        /// Gets or sets the name of the machine that will be used within Octopus to identify this machine.
        /// </summary>
        string MachineName { get; set; }

        /// <summary>
        /// Gets or sets the machine policy that applies to this machine.
        /// </summary>
        string MachinePolicy { get; set; }

        /// <summary>
        /// Gets or sets the hostname that Octopus should use when communicating with the Tentacle.
        /// </summary>
        string TentacleHostname { get; set; }

        /// <summary>
        /// Gets or sets the TCP port that Octopus should use when communicating with the Tentacle.
        /// </summary>
        int TentaclePort { get; set; }

        /// <summary>
        /// Gets or sets the certificate thumbprint that Octopus should expect when communicating with the Tentacle.
        /// </summary>
        string TentacleThumbprint { get; set; }

        /// <summary>
        /// If a machine with the same name already exists, it won't be overwritten by default (instead, an
        /// <see cref="ArgumentException" /> will be thrown).
        /// Set this property to <c>true</c> if you do want the existing machine to be overwritten.
        /// </summary>
        bool AllowOverwrite { get; set; }

        /// <summary>
        /// The communication style to use with the Tentacle. Allowed values are: TentacleActive, in which case the
        /// Tentacle will connect to the Octopus server for instructions; or, TentaclePassive, in which case the
        /// Tentacle will listen for commands from the server (default).
        /// </summary>
        CommunicationStyle CommunicationStyle { get; set; }

        Uri SubscriptionId { get; set; }

#if SYNC_CLIENT
        /// <summary>
        /// Executes the operation against the specified Octopus Deploy server.
        /// </summary>
        /// <param name="serverEndpoint">The Octopus Deploy server endpoint.</param>
        void Execute(OctopusServerEndpoint serverEndpoint);

        /// <summary>
        /// Executes the operation against the specified Octopus Deploy server.
        /// </summary>
        /// <param name="repository">The Octopus Deploy repository.</param>
        void Execute(OctopusRepository repository);

        /// <summary>
        /// Executes the operation against the specified Octopus Deploy server.
        /// </summary>
        /// <param name="repository">The Octopus Deploy repository.</param>
        void Execute(IOctopusRepository repository);
#endif
        /// <summary>
        /// Executes the operation against the specified Octopus Deploy server.
        /// </summary>
        /// <param name="serverEndpoint">The Octopus Deploy server endpoint.</param>
        Task ExecuteAsync(OctopusServerEndpoint serverEndpoint);

        /// <summary>
        /// Executes the operation against the specified Octopus Deploy server.
        /// </summary>
        /// <param name="repository">The Octopus Deploy repository.</param>
        Task ExecuteAsync(OctopusAsyncRepository repository);

        /// <summary>
        /// Executes the operation against the specified Octopus Deploy server.
        /// </summary>
        /// <param name="repository">The Octopus Deploy repository.</param>
        Task ExecuteAsync(IOctopusAsyncRepository repository);
    }
}