﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Client.Util;
using IEnvironmentRepository = Octopus.Client.Repositories.IEnvironmentRepository;
using IAsyncEnvironmentRepository = Octopus.Client.Repositories.Async.IEnvironmentRepository;
using ITenantRepository = Octopus.Client.Repositories.ITenantRepository;
using IAsyncTenantRepository = Octopus.Client.Repositories.Async.ITenantRepository;

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
            List<TenantResource> tenants = new();            
            if (Tenants is not null && Tenants.Any())
            {
                var tenantsByNameIdOrSlug =
                    repository.Tenants.FindByNameIdOrSlugs<TenantResource, ITenantRepository>(
                        Tenants, missing => CouldNotFindByMultipleMessage("tenant", missing.ToArray()));
                tenants.AddRange(tenantsByNameIdOrSlug);
            }
            return tenants;
        }

        void ValidateTenantTags(IOctopusSpaceRepository repository)
        {
            if (TenantTags == null || !TenantTags.Any())
                return;

            var tagSets = repository.TagSets.FindAll();
            var missingTags = TenantTags.Where(tt =>
                    !tagSets.Any(ts =>
                        ts.Tags.Any(t => t.CanonicalTagName.Equals(tt, StringComparison.OrdinalIgnoreCase))))
                .ToList();

            if (missingTags.Any())
                throw new InvalidRegistrationArgumentsException(CouldNotFindByNameMessage("tag", missingTags.ToArray()));
        }

        List<EnvironmentResource> GetEnvironments(IOctopusSpaceRepository repository)
        {
            List<EnvironmentResource> environments = new();
            if (EnvironmentNames is not null && EnvironmentNames.Any())
            {
                var envsByName = repository.Environments.FindByNames(EnvironmentNames);
                environments.AddRange(envsByName);

                //if there are any missing environment names only, then we want to throw an exception and not check the missing names against slugs or ids
                var missingByNameOnly = EnvironmentNames
                    .Except(envsByName.Select(e => e.Name), StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                if (missingByNameOnly.Any())
                    throw new InvalidRegistrationArgumentsException(CouldNotFindByNameMessage("environment", missingByNameOnly.ToArray()));
            }

            if (Environments is not null && Environments.Any())
            {
                var environmentsByNameIdOrSlug =
                    repository.Environments.FindByNameIdOrSlugs<EnvironmentResource, IEnvironmentRepository>(
                        Environments, missing => CouldNotFindByMultipleMessage("environment", missing.ToArray()));
                environments.AddRange(environmentsByNameIdOrSlug);
            }

            return environments;
        }

        MachineResource GetMachine(IOctopusSpaceRepository repository)
        {
            var existing = default(MachineResource);
            try
            {
                existing = repository.Machines.FindByName(MachineName);
            }
            catch (OctopusDeserializationException)
            {
                // probably caused by resource incompatibility between versions
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
            List<TenantResource> tenants = new();            
            if (Tenants is not null && Tenants.Any())
            {
                var tenantsByNameIdOrSlug =
                    await repository.Tenants.FindByNameIdOrSlugs<TenantResource, IAsyncTenantRepository>(
                        Tenants, missing => CouldNotFindByMultipleMessage("tenant", missing.ToArray()));
                tenants.AddRange(tenantsByNameIdOrSlug);
            }
            return tenants;
        }

        async Task ValidateTenantTags(IOctopusSpaceAsyncRepository repository)
        {
            if (TenantTags == null || !TenantTags.Any())
                return;

            var tagSets = await repository.TagSets.FindAll().ConfigureAwait(false);
            var missingTags = TenantTags.Where(tt =>
                    !tagSets.Any(ts =>
                        ts.Tags.Any(t => t.CanonicalTagName.Equals(tt, StringComparison.OrdinalIgnoreCase))))
                .ToList();

            if (missingTags.Any())
                throw new InvalidRegistrationArgumentsException(CouldNotFindByNameMessage("tag", missingTags.ToArray()));
        }

        async Task<List<EnvironmentResource>> GetEnvironments(IOctopusSpaceAsyncRepository repository)
        {
            List<EnvironmentResource> environments = new();
            if (EnvironmentNames is not null && EnvironmentNames.Any())
            {
                var envsByName = await repository.Environments.FindByNames(EnvironmentNames).ConfigureAwait(false);
                environments.AddRange(envsByName);

                //if there are any missing environment names only, then we want to throw an exception and not check the missing names against slugs or ids
                var missingByNameOnly = EnvironmentNames
                    .Except(envsByName.Select(e => e.Name), StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                if (missingByNameOnly.Any())
                    throw new InvalidRegistrationArgumentsException(CouldNotFindByNameMessage("environment", missingByNameOnly.ToArray()));
            }

            if (Environments is not null && Environments.Any())
            {
                var environmentsByNameIdOrSlug =
                    await repository.Environments.FindByNameIdOrSlugs<EnvironmentResource, IAsyncEnvironmentRepository>(
                        Environments, missing => CouldNotFindByMultipleMessage("environment", missing.ToArray()));
                environments.AddRange(environmentsByNameIdOrSlug);
            }

            return environments;
        }

        async Task<MachineResource> GetMachine(IOctopusSpaceAsyncRepository repository)
        {
            var existing = default(MachineResource);
            try
            {
                existing = await repository.Machines.FindByName(MachineName).ConfigureAwait(false);
            }
            catch (OctopusDeserializationException)
            {
                // probably caused by resource incompatability between versions
            }

            return existing ?? new MachineResource();
        }

        void ApplyDeploymentTargetChanges(MachineResource machine, IEnumerable<EnvironmentResource> environment,
            IEnumerable<TenantResource> tenants)
        {
            machine.EnvironmentIds = new ReferenceCollection(environment.Select(e => e.Id).ToArray());
            machine.TenantIds = new ReferenceCollection(tenants.Select(t => t.Id).ToArray());
            machine.TenantTags = new ReferenceCollection(TenantTags);
            machine.Roles = new ReferenceCollection(Roles);
            machine.TenantedDeploymentParticipation = TenantedDeploymentParticipation;
        }
    }
}