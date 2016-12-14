using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Operations
{
    /// <summary>
    /// Encapsulates the operation for registering machines.
    /// </summary>
    public class RegisterMachineOperation : IRegisterMachineOperation
    {
        readonly IOctopusClientFactory clientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterMachineOperation" /> class.
        /// </summary>
        public RegisterMachineOperation() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterMachineOperation" /> class.
        /// </summary>
        /// <param name="clientFactory">The client factory.</param>
        public RegisterMachineOperation(IOctopusClientFactory clientFactory)
        {
            this.clientFactory = clientFactory ?? new OctopusClientFactory();
        }

        /// <summary>
        /// Gets or sets the environments that this machine should be added to.
        /// </summary>
        public string[] EnvironmentNames { get; set; }

        /// <summary>
        /// Gets or sets the roles that this machine belongs to.
        /// </summary>
        public string[] Roles { get; set; }

        /// <summary>
        /// Gets or sets the tenants that this machine is linked to.
        /// </summary>
        public string[] Tenants { get; set; }

        /// <summary>
        /// Gets or sets the tenant tags that this machine is linked to.
        /// </summary>
        public string[] TenantTags { get; set; }

        /// <summary>
        /// Gets or sets the name of the machine that will be used within Octopus to identify this machine.
        /// </summary>
        public string MachineName { get; set; }

        /// <summary>
        /// Get or sets the machine policy that applied to this machine.
        /// </summary>
        public string MachinePolicy { get; set; }

        /// <summary>
        /// Gets or sets the hostname that Octopus should use when communicating with the Tentacle.
        /// </summary>
        public string TentacleHostname { get; set; }

        /// <summary>
        /// Gets or sets the TCP port that Octopus should use when communicating with the Tentacle.
        /// </summary>
        public int TentaclePort { get; set; }

        /// <summary>
        /// Gets or sets the certificate thumbprint that Octopus should expect when communicating with the Tentacle.
        /// </summary>
        public string TentacleThumbprint { get; set; }

        /// <summary>
        /// If a machine with the same name already exists, it won't be overwritten by default (instead, an
        /// <see cref="ArgumentException" /> will be thrown).
        /// Set this property to <c>true</c> if you do want the existing machine to be overwritten.
        /// </summary>
        public bool AllowOverwrite { get; set; }

        /// <summary>
        /// The communication style to use with the Tentacle. Allowed values are: TentacleActive, in which case the
        /// Tentacle will connect to the Octopus server for instructions; or, TentaclePassive, in which case the
        /// Tentacle will listen for commands from the server (default).
        /// </summary>
        public CommunicationStyle CommunicationStyle { get; set; }

        public Uri SubscriptionId { get; set; }

#if SYNC_CLIENT
        /// <summary>
        /// Executes the operation against the specified Octopus Deploy server.
        /// </summary>
        /// <param name="serverEndpoint">The Octopus Deploy server endpoint.</param>
        /// <exception cref="System.ArgumentException">
        /// </exception>
        public void Execute(OctopusServerEndpoint serverEndpoint)
        {
            using (var client = clientFactory.CreateClient(serverEndpoint))
            {
                var repository = new OctopusRepository(client);

                Execute(repository);
            }
        }

        /// <summary>
        /// Executes the operation against the specified Octopus Deploy server.
        /// </summary>
        /// <param name="repository">The Octopus Deploy server repository.</param>
        /// <exception cref="System.ArgumentException">
        /// </exception>
        public void Execute(OctopusRepository repository)
        {
            var selectedEnvironments = GetEnvironments(repository);
            var machinePolicy = GetMachinePolicy(repository);
            var machine = GetMachine(repository);
            var tenants = GetTenants(repository);
            ValidateTenantTags(repository);

            ApplyChanges(machine, selectedEnvironments, machinePolicy, tenants);

            if (machine.Id != null)
                repository.Machines.Modify(machine);
            else
                repository.Machines.Create(machine);
        }

        List<TenantResource> GetTenants(OctopusRepository repository)
        {
            if (Tenants == null || !Tenants.Any())
            {
                return new List<TenantResource>();
            }
            var tenantsByName = repository.Tenants.FindByNames(Tenants);
            var missing = Tenants.Except(tenantsByName.Select(e => e.Name), StringComparer.OrdinalIgnoreCase).ToArray();


            var tenantsById = repository.Tenants.Get(missing);
            missing = missing.Except(tenantsById.Select(e => e.Id), StringComparer.OrdinalIgnoreCase).ToArray();

            if (missing.Any())
                throw new ArgumentException($"Could not find the {"tenant" + (missing.Length == 1 ? "" : "s")} {string.Join(", ", missing)} on the Octopus server.");

            return tenantsById.Concat(tenantsByName).ToList();
        }

        void ValidateTenantTags(OctopusRepository repository)
        {
            if (TenantTags == null || !TenantTags.Any())
                return;

            var tagSets = repository.TagSets.FindAll();
            var missingTags = TenantTags.Where(tt => !tagSets.Any(ts => ts.Tags.Any(t => t.CanonicalTagName.Equals(tt, StringComparison.OrdinalIgnoreCase)))).ToList();

            if (missingTags.Any())
                throw new ArgumentException($"Could not find the {"tag" + (missingTags.Count == 1 ? "" : "s")} {string.Join(", ", missingTags)} on the Octopus server.");
        }


        List<EnvironmentResource> GetEnvironments(OctopusRepository repository)
        {
            var selectedEnvironments = repository.Environments.FindByNames(EnvironmentNames);

            var missing = EnvironmentNames.Except(selectedEnvironments.Select(e => e.Name), StringComparer.OrdinalIgnoreCase).ToList();

            if (missing.Any())
                throw new ArgumentException($"Could not find the {"environment" + (missing.Count == 1 ? "" : "s")} {string.Join(", ", missing)} on the Octopus server.");

            return selectedEnvironments;
        }

        MachinePolicyResource GetMachinePolicy(OctopusRepository repository)
        {

            var machinePolicy = default(MachinePolicyResource);
            if (!string.IsNullOrEmpty(MachinePolicy))
            {
                machinePolicy = repository.MachinePolicies.FindByName(MachinePolicy);
                if (machinePolicy == null)
                    throw new ArgumentException($"Could not find a machine policy named {MachinePolicy}.");
            }
            return machinePolicy;
        }

        MachineResource GetMachine(OctopusRepository repository)
        {
            var existing = default(MachineResource);
            try
            {
                existing = repository.Machines.FindByName(MachineName);
                if (!AllowOverwrite && existing?.Id != null)
                    throw new ArgumentException(string.Format("A machine named '{0}' already exists in the environment. Use the 'force' parameter if you intended to update the existing machine.", MachineName));
            }
            catch (OctopusDeserializationException) // eat it, probably caused by resource incompatability between versions
            {
            }
            return existing ?? new MachineResource();
        }
#endif

        /// <summary>
        /// Executes the operation against the specified Octopus Deploy server.
        /// </summary>
        /// <param name="serverEndpoint">The Octopus Deploy server endpoint.</param>
        /// <exception cref="System.ArgumentException">
        /// </exception>
        public async Task ExecuteAsync(OctopusServerEndpoint serverEndpoint)
        {
            using (var client = await clientFactory.CreateAsyncClient(serverEndpoint).ConfigureAwait(false))
            {
                var repository = new OctopusAsyncRepository(client);

                await ExecuteAsync(repository).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Executes the operation against the specified Octopus Deploy server.
        /// </summary>
        /// <param name="repository">The Octopus Deploy server repository.</param>
        /// <exception cref="System.ArgumentException">
        /// </exception>
        public async Task ExecuteAsync(OctopusAsyncRepository repository)
        {
            var selectedEnvironments = GetEnvironments(repository).ConfigureAwait(false);
            var machinePolicy = GetMachinePolicy(repository).ConfigureAwait(false);
            var machineTask = GetMachine(repository).ConfigureAwait(false);
            var tenants = GetTenants(repository).ConfigureAwait(false);
            await ValidateTenantTags(repository).ConfigureAwait(false);

            var machine = await machineTask;
            ApplyChanges(machine, await selectedEnvironments, await machinePolicy, await tenants);

            if (machine.Id != null)
                await repository.Machines.Modify(machine).ConfigureAwait(false);
            else
                await repository.Machines.Create(machine).ConfigureAwait(false);
        }

        async Task<List<TenantResource>> GetTenants(OctopusAsyncRepository repository)
        {
            if (Tenants == null || !Tenants.Any())
            {
                return new List<TenantResource>();
            }
            var tenantsByName = await repository.Tenants.FindByNames(Tenants).ConfigureAwait(false);
            var missing = Tenants.Except(tenantsByName.Select(e => e.Name), StringComparer.OrdinalIgnoreCase).ToArray();


            var tenantsById = await repository.Tenants.Get(missing).ConfigureAwait(false);
            missing = missing.Except(tenantsById.Select(e => e.Id), StringComparer.OrdinalIgnoreCase).ToArray();

            if (missing.Any())
                throw new ArgumentException($"Could not find the {"tenant" + (missing.Length == 1 ? "" : "s")} {string.Join(", ", missing)} on the Octopus server.");

            return tenantsById.Concat(tenantsByName).ToList();
        }

        async Task ValidateTenantTags(OctopusAsyncRepository repository)
        {
            if (TenantTags == null || !TenantTags.Any())
                return;

            var tagSets = await repository.TagSets.FindAll().ConfigureAwait(false);
            var missingTags = TenantTags.Where(tt => !tagSets.Any(ts => ts.Tags.Any(t => t.CanonicalTagName.Equals(tt, StringComparison.OrdinalIgnoreCase)))).ToList();

            if (missingTags.Any())
                throw new ArgumentException($"Could not find the {"tag" + (missingTags.Count == 1 ? "" : "s")} {string.Join(", ", missingTags)} on the Octopus server.");
        }

        async Task<List<EnvironmentResource>> GetEnvironments(OctopusAsyncRepository repository)
        {
            var selectedEnvironments = await repository.Environments.FindByNames(EnvironmentNames).ConfigureAwait(false);

            var missing = EnvironmentNames.Except(selectedEnvironments.Select(e => e.Name), StringComparer.OrdinalIgnoreCase).ToList();

            if (missing.Any())
                throw new ArgumentException($"Could not find the {"environment" + (missing.Count == 1 ? "" : "s")} {string.Join(", ", missing)} on the Octopus server.");

            return selectedEnvironments;
        }

        async Task<MachinePolicyResource> GetMachinePolicy(OctopusAsyncRepository repository)
        {

            var machinePolicy = default(MachinePolicyResource);
            if (!string.IsNullOrEmpty(MachinePolicy))
            {
                machinePolicy = await repository.MachinePolicies.FindByName(MachinePolicy).ConfigureAwait(false);
                if (machinePolicy == null)
                    throw new ArgumentException($"Could not find a machine policy named {MachinePolicy}.");
            }
            return machinePolicy;
        }

        async Task<MachineResource> GetMachine(OctopusAsyncRepository repository)
        {
            var existing = default(MachineResource);
            try
            {
                existing = await repository.Machines.FindByName(MachineName).ConfigureAwait(false);
                if (!AllowOverwrite && existing?.Id != null)
                    throw new ArgumentException(string.Format("A machine named '{0}' already exists in the environment. Use the 'force' parameter if you intended to update the existing machine.", MachineName));
            }
            catch (OctopusDeserializationException) // eat it, probably caused by resource incompatability between versions
            {
            }
            return existing ?? new MachineResource();
        }

        void ApplyChanges(MachineResource machine, IEnumerable<EnvironmentResource> environment, MachinePolicyResource machinePolicy, IEnumerable<TenantResource> tenants)
        {
            machine.EnvironmentIds = new ReferenceCollection(environment.Select(e => e.Id).ToArray());
            machine.TenantIds = new ReferenceCollection(tenants.Select(t => t.Id).ToArray());
            machine.TenantTags = new ReferenceCollection(TenantTags);
            machine.Roles = new ReferenceCollection(Roles);
            machine.Name = MachineName;
            machine.MachinePolicyId = machinePolicy?.Id;

            if (CommunicationStyle == CommunicationStyle.TentaclePassive)
            {
                var listening = new ListeningTentacleEndpointResource();
                listening.Uri = new Uri("https://" + TentacleHostname.ToLowerInvariant() + ":" + TentaclePort.ToString(CultureInfo.InvariantCulture) + "/").ToString();
                listening.Thumbprint = TentacleThumbprint;
                machine.Endpoint = listening;
            }
            else if (CommunicationStyle == CommunicationStyle.TentacleActive)
            {
                var polling = new PollingTentacleEndpointResource();
                polling.Uri = SubscriptionId.ToString();
                polling.Thumbprint = TentacleThumbprint;
                machine.Endpoint = polling;
            }
        }
    }
}