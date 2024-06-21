using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;

namespace Octopus.Client.Operations
{
    /// <summary>
    /// Encapsulates the operation for registering machines.
    /// </summary>
    public class RegisterWorkerOperation : RegisterMachineOperationBase, IRegisterWorkerOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterWorkerOperation" /> class.
        /// </summary>
        public RegisterWorkerOperation() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterWorkerOperation" /> class.
        /// </summary>
        /// <param name="clientFactory">The client factory.</param>
        public RegisterWorkerOperation(IOctopusClientFactory clientFactory) : base(clientFactory)
        {

        }

        /// <summary>
        /// Gets or sets the worker pools that this machine should be added to.
        /// </summary>
        [Obsolete($"Use the {nameof(WorkerPools)} property as it supports worker pool names, slugs and Ids.")]
        public string[] WorkerPoolNames { get; set; }
        
        /// <summary>
        /// Gets or sets the worker pools that this machine should be added to. These can be worker pool names, slugs or Ids
        /// </summary>
        public string[] WorkerPools { get; set; }

        /// <summary>
        /// Executes the operation against the specified Octopus Deploy server.
        /// </summary>
        /// <param name="repository">The Octopus Deploy server repository.</param>
        /// <exception cref="InvalidRegistrationArgumentsException">
        /// </exception>
        public override void Execute(IOctopusSpaceRepository repository)
        {
            var selectedPools = GetWorkerPools(repository);
            var machinePolicy = GetMachinePolicy(repository);
            var machine = GetWorker(repository);
            var proxy = GetProxy(repository);

            ApplyBaseChanges(machine, machinePolicy, proxy);

            machine.WorkerPoolIds = new ReferenceCollection(selectedPools.Select(p => p.Id).ToArray());

            if (machine.Id != null)
                repository.Workers.Modify(machine);
            else
                repository.Workers.Create(machine);
        }

        List<WorkerPoolResource> GetWorkerPools(IOctopusSpaceRepository repository)
        {
            var selectedPools = repository.WorkerPools.FindByNames(WorkerPoolNames);

            var missing = WorkerPoolNames.Except(selectedPools.Select(p => p.Name), StringComparer.OrdinalIgnoreCase).ToList();

            if (missing.Any())
                throw new InvalidRegistrationArgumentsException(CouldNotFindByNameMessage("worker pool", missing.ToArray()));

            return selectedPools;
        }

        WorkerResource GetWorker(IOctopusSpaceRepository repository)
        {
            var existing = default(WorkerResource);
            try
            {
                existing = repository.Workers.FindByName(MachineName);
                if (!AllowOverwrite && existing?.Id != null)
                    throw new InvalidRegistrationArgumentsException($"A worker named '{MachineName}' already exists. Use the 'force' parameter if you intended to update the existing machine.");
            }
            catch (OctopusDeserializationException) // eat it, probably caused by resource incompatability between versions
            {
            }
            return existing ?? new WorkerResource();
        }

        /// <summary>
        /// Executes the operation against the specified Octopus Deploy server.
        /// </summary>
        /// <param name="repository">The Octopus Deploy server repository.</param>
        /// <exception cref="InvalidRegistrationArgumentsException">
        /// </exception>
        public override async Task ExecuteAsync(IOctopusSpaceAsyncRepository repository)
        {
            var selectedPools = GetWorkerPools(repository).ConfigureAwait(false);
            var machinePolicy = GetMachinePolicy(repository).ConfigureAwait(false);
            var machineTask = GetWorker(repository).ConfigureAwait(false);
            var proxy = GetProxy(repository).ConfigureAwait(false);

            var machine = await machineTask;
            ApplyBaseChanges(machine, await machinePolicy, await proxy);

            machine.WorkerPoolIds = new ReferenceCollection((await selectedPools).Select(p => p.Id).ToArray());

            if (machine.Id != null)
                await repository.Workers.Modify(machine).ConfigureAwait(false);
            else
                await repository.Workers.Create(machine).ConfigureAwait(false);
        }

        async Task<List<WorkerPoolResource>> GetWorkerPools(IOctopusSpaceAsyncRepository repository)
        {
            List<WorkerPoolResource> workerPools = new();
            if (WorkerPoolNames is not null && WorkerPoolNames.Any())
            {
                var workerPoolsByName = await repository.WorkerPools.FindByNames(WorkerPoolNames).ConfigureAwait(false);
                workerPools.AddRange(workerPoolsByName);

                var missingByNameOnly = WorkerPoolNames.Except(workerPoolsByName.Select(p => p.Name), StringComparer.OrdinalIgnoreCase).ToList();

                if (missingByNameOnly.Any())
                    throw new InvalidRegistrationArgumentsException(CouldNotFindByNameMessage("worker pool", missingByNameOnly.ToArray()));
            }
            
            if (WorkerPools is not null && WorkerPools.Any())
            {
                var workerPoolsByName = await repository.WorkerPools.FindByNames(WorkerPools).ConfigureAwait(false);
                workerPools.AddRange(workerPoolsByName);

                var missing = WorkerPools
                    .Except(workerPoolsByName.Select(e => e.Name), StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                //use the missing names to try and find by slug
                var workerPoolsBySlug = await repository.WorkerPools.FindBySlugs(missing, CancellationToken.None)
                    .ConfigureAwait(false);
                workerPools.AddRange(workerPoolsBySlug);

                missing = missing
                    .Except(workerPoolsBySlug.Select(e => e.Slug), StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                //any other missing slugs/names could be Id's, so looks again
                var workerPoolsByIds = await repository.WorkerPools.Get(missing).ConfigureAwait(false);
                workerPools.AddRange(workerPoolsByIds);

                missing = missing
                    .Except(workerPoolsByIds.Select(e => e.Id), StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                if (missing.Any())
                    throw new InvalidRegistrationArgumentsException(CouldNotFindByMultipleMessage("worker pool", missing.ToArray()));
            }
            
            return workerPools;
        }

        async Task<WorkerResource> GetWorker(IOctopusSpaceAsyncRepository repository)
        {
            var existing = default(WorkerResource);
            try
            {
                existing = await repository.Workers.FindByName(MachineName).ConfigureAwait(false);
                if (!AllowOverwrite && existing?.Id != null)
                    throw new InvalidRegistrationArgumentsException($"A worker named '{MachineName}' already exists. Use the 'force' parameter if you intended to update the existing machine.");
            }
            catch (OctopusDeserializationException) // eat it, probably caused by resource incompatability between versions
            {
            }
            return existing ?? new WorkerResource();
        }
    }
}
