﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;

namespace Octopus.Client.Operations
{
    /// <summary>
    /// Encapsulates the operation for registering machines.
    /// </summary>
    public class RegisterMachineOperation : RegisterMachineOperationBase, IRegisterMachineOperation
    {
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
        public RegisterMachineOperation(IOctopusClientFactory clientFactory) : base(clientFactory)
        {

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
        /// How the machine should participate in Tenanted Deployments.
        /// Allowed values are Untenanted, TenantedOrUntenanted or Tenanted
        /// </summary>
        public TenantedDeploymentMode TenantedDeploymentParticipation { get; set; }

        /// <summary>
        /// Executes the operation against the specified Octopus Deploy server.
        /// </summary>
        /// <param name="repository">The Octopus Deploy server repository.</param>
        /// <exception cref="InvalidRegistrationArgumentsException">
        /// </exception>
        public override void Execute(IOctopusSpaceRepository repository)
        {
            var machine = GetMachine(repository);
            var proxy = GetProxy(repository);

            if (!IsExistingMachine(machine) || AllowOverwrite)
            {
                var machinePolicy = GetMachinePolicy(repository);
                ValidateTenantTags(repository);
                ApplyBaseChanges(machine, machinePolicy, proxy);
                ApplyDeploymentTargetChanges(machine, GetEnvironments(repository), GetTenants(repository));
            }
            else
            {
                PrepareMachineForReRegistration(machine, proxy?.Id);
            }

            ModifyOrCreateMachine(repository, machine);
        }

        static bool IsExistingMachine(MachineResource machine)
        {
            return machine.Id != null;
        }

        static void ModifyOrCreateMachine(IOctopusSpaceRepository repository, MachineResource machine)
        {
            if (IsExistingMachine(machine))
                repository.Machines.Modify(machine);
            else
                repository.Machines.Create(machine);
        }

        List<TenantResource> GetTenants(IOctopusSpaceRepository repository)
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
                throw new InvalidRegistrationArgumentsException(CouldNotFindMessage("tenant", missing));

            return tenantsById.Concat(tenantsByName).ToList();
        }

        void ValidateTenantTags(IOctopusSpaceRepository repository)
        {
            if (TenantTags == null || !TenantTags.Any())
                return;

            var tagSets = repository.TagSets.FindAll();
            var missingTags = TenantTags.Where(tt => !tagSets.Any(ts => ts.Tags.Any(t => t.CanonicalTagName.Equals(tt, StringComparison.OrdinalIgnoreCase)))).ToList();

            if (missingTags.Any())
                throw new InvalidRegistrationArgumentsException(CouldNotFindMessage("tag", missingTags.ToArray()));
        }

        List<EnvironmentResource> GetEnvironments(IOctopusSpaceRepository repository)
        {
            var selectedEnvironments = repository.Environments.FindByNames(EnvironmentNames);

            var missing = EnvironmentNames.Except(selectedEnvironments.Select(e => e.Name), StringComparer.OrdinalIgnoreCase).ToList();

            if (missing.Any())
                throw new InvalidRegistrationArgumentsException(CouldNotFindMessage("environment", missing.ToArray()));

            return selectedEnvironments;
        }

        MachineResource GetMachine(IOctopusSpaceRepository repository)
        {
            var existing = default(MachineResource);
            try
            {
                existing = repository.Machines.FindByName(MachineName);
            }
            catch (OctopusDeserializationException) // eat it, probably caused by resource incompatibility between versions
            {
            }

            return existing ?? new MachineResource();
        }

        /// <summary>
        /// Executes the operation against the specified Octopus Deploy server.
        /// </summary>
        /// <param name="repository">The Octopus Deploy server repository.</param>
        /// <exception cref="InvalidRegistrationArgumentsException">
        /// </exception>
        public override async Task ExecuteAsync(IOctopusSpaceAsyncRepository repository)
        {
            var machine = await GetMachine(repository).ConfigureAwait(false);
            var proxy = await GetProxy(repository).ConfigureAwait(false);

            if (!IsExistingMachine(machine) || AllowOverwrite)
            {
                var machinePolicy = GetMachinePolicy(repository).ConfigureAwait(false);
                await ValidateTenantTags(repository).ConfigureAwait(false);
                ApplyBaseChanges(machine, await machinePolicy, proxy);
                var selectedEnvironments = await GetEnvironments(repository).ConfigureAwait(false);
                var tenants = await GetTenants(repository).ConfigureAwait(false);
                ApplyDeploymentTargetChanges(machine, selectedEnvironments, tenants);
            }
            else
            {
                PrepareMachineForReRegistration(machine, proxy?.Id);
            }

            await ModifyOrCreateMachine(repository, machine).ConfigureAwait(false);
        }

        protected virtual void PrepareMachineForReRegistration(MachineResource machineResource, string proxyId)
        {
            throw new InvalidRegistrationArgumentsException(
                $"A machine named '{MachineName}' already exists in the environment. Use the 'force' parameter if you intended to update the existing machine.");
        }

        static async Task ModifyOrCreateMachine(IOctopusSpaceAsyncRepository repository, MachineResource machine)
        {
            if (IsExistingMachine(machine))
                await repository.Machines.Modify(machine).ConfigureAwait(false);
            else
                await repository.Machines.Create(machine).ConfigureAwait(false);
        }

        async Task<List<TenantResource>> GetTenants(IOctopusSpaceAsyncRepository repository)
        {
            if (Tenants == null || !Tenants.Any())
            {
                return new List<TenantResource>();
            }

            var tenantsByName = new List<TenantResource>();
            foreach (var tenantName in Tenants)
            {
                var tenant = await repository.Tenants.FindByName(tenantName).ConfigureAwait(false);
                if (tenant != null)
                    tenantsByName.Add(tenant);
            }

            var missing = Tenants.Except(tenantsByName.Select(e => e.Name), StringComparer.OrdinalIgnoreCase).ToArray();

            var tenantsById = await repository.Tenants.Get(missing).ConfigureAwait(false);
            missing = missing.Except(tenantsById.Select(e => e.Id), StringComparer.OrdinalIgnoreCase).ToArray();

            if (missing.Any())
                throw new InvalidRegistrationArgumentsException(CouldNotFindMessage("tenant", missing));

            return tenantsById.Concat(tenantsByName).ToList();
        }

        async Task ValidateTenantTags(IOctopusSpaceAsyncRepository repository)
        {
            if (TenantTags == null || !TenantTags.Any())
                return;

            var tagSets = await repository.TagSets.FindAll().ConfigureAwait(false);
            var missingTags = TenantTags.Where(tt => !tagSets.Any(ts => ts.Tags.Any(t => t.CanonicalTagName.Equals(tt, StringComparison.OrdinalIgnoreCase)))).ToList();

            if (missingTags.Any())
                throw new InvalidRegistrationArgumentsException(CouldNotFindMessage("tag", missingTags.ToArray()));
        }

        async Task<List<EnvironmentResource>> GetEnvironments(IOctopusSpaceAsyncRepository repository)
        {
            var selectedEnvironments = new List<EnvironmentResource>();
            foreach (var environmentName in EnvironmentNames)
            {
                var environment = await repository.Environments.FindByName(environmentName).ConfigureAwait(false);
                if (environment != null)
                    selectedEnvironments.Add(environment);
            }

            var missing = EnvironmentNames.Except(selectedEnvironments.Select(e => e.Name), StringComparer.OrdinalIgnoreCase).ToList();

            if (missing.Any())
                throw new InvalidRegistrationArgumentsException(CouldNotFindMessage("environment", missing.ToArray()));

            return selectedEnvironments;
        }

        async Task<MachineResource> GetMachine(IOctopusSpaceAsyncRepository repository)
        {
            var existing = default(MachineResource);
            try
            {
                existing = await repository.Machines.FindByName(MachineName).ConfigureAwait(false);
            }
            catch (OctopusDeserializationException) // eat it, probably caused by resource incompatability between versions
            {
            }
            return existing ?? new MachineResource();
        }

        void ApplyDeploymentTargetChanges(MachineResource machine, IEnumerable<EnvironmentResource> environment, IEnumerable<TenantResource> tenants)
        {
            machine.EnvironmentIds = new ReferenceCollection(environment.Select(e => e.Id).ToArray());
            machine.TenantIds = new ReferenceCollection(tenants.Select(t => t.Id).ToArray());
            machine.TenantTags = new ReferenceCollection(TenantTags);
            machine.Roles = new ReferenceCollection(Roles);
            machine.TenantedDeploymentParticipation = TenantedDeploymentParticipation;
        }
    }
}
