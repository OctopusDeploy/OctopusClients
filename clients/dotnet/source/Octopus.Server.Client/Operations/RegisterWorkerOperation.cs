using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Client.Util;
using IWorkerPoolRepository = Octopus.Client.Repositories.IWorkerPoolRepository;
using IAsyncWorkerPoolRepository = Octopus.Client.Repositories.Async.IWorkerPoolRepository;

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
            var worker = GetWorker(repository);
            var proxy = GetProxy(repository);

            if (!IsExistingWorker(worker) || AllowOverwrite)
            {
                var machinePolicy = GetMachinePolicy(repository);
                ApplyBaseChanges(worker, machinePolicy, proxy);
                var selectedPools = GetWorkerPools(repository);
                worker.WorkerPoolIds = new ReferenceCollection(selectedPools.Select(p => p.Id).ToArray());
            }
            else
            {
                PrepareWorkerForReRegistration(worker, proxy?.Id);
            }
            
            ModifyOrCreateWorker(repository, worker);
        }
        
        /// <summary>
        /// Executes the operation against the specified Octopus Deploy server.
        /// </summary>
        /// <param name="repository">The Octopus Deploy server repository.</param>
        /// <exception cref="InvalidRegistrationArgumentsException">
        /// </exception>
        public override async Task ExecuteAsync(IOctopusSpaceAsyncRepository repository)
        {
            var worker = await GetWorker(repository).ConfigureAwait(false);
            var proxy = await GetProxy(repository).ConfigureAwait(false);

            if (!IsExistingWorker(worker) || AllowOverwrite)
            {
                var machinePolicy = await GetMachinePolicy(repository).ConfigureAwait(false);
                ApplyBaseChanges(worker, machinePolicy, proxy);
                var selectedPools = GetWorkerPools(repository).ConfigureAwait(false);
                worker.WorkerPoolIds = new ReferenceCollection((await selectedPools).Select(p => p.Id).ToArray());
            }
            else
            {
                PrepareWorkerForReRegistration(worker, proxy?.Id);
            }

            await ModifyOrCreateWorker(repository, worker);
        }
        
        static bool IsExistingWorker(WorkerResource worker)
        {
            return worker.Id != null;
        }

        protected virtual void PrepareWorkerForReRegistration(WorkerResource workerResource, string proxyId)
        {
            throw new InvalidRegistrationArgumentsException(
                $"A worker named '{MachineName}' already exists in the environment. Use the 'force' parameter if you intended to update the existing worker.");
        }
        
        WorkerResource GetWorker(IOctopusSpaceRepository repository)
        {
            var existing = default(WorkerResource);
            try
            {
                existing = repository.Workers.FindByName(MachineName);
            }
            catch (OctopusDeserializationException) // eat it, probably caused by resource incompatability between versions
            {
            }
            return existing ?? new WorkerResource();
        }
        
        async Task<WorkerResource> GetWorker(IOctopusSpaceAsyncRepository repository)
        {
            var existing = default(WorkerResource);
            try
            {
                existing = await repository.Workers.FindByName(MachineName).ConfigureAwait(false);
            }
            catch (OctopusDeserializationException) // eat it, probably caused by resource incompatability between versions
            {
            }
            return existing ?? new WorkerResource();
        }

        List<WorkerPoolResource> GetWorkerPools(IOctopusSpaceRepository repository)
        {
            List<WorkerPoolResource> workerPools = new();
            if (WorkerPoolNames is not null && WorkerPoolNames.Any())
            {
                var workerPoolsByName = repository.WorkerPools.FindByNames(WorkerPoolNames);
                workerPools.AddRange(workerPoolsByName);

                var missingByNameOnly = WorkerPoolNames.Except(workerPoolsByName.Select(p => p.Name), StringComparer.OrdinalIgnoreCase).ToList();

                if (missingByNameOnly.Any())
                    throw new InvalidRegistrationArgumentsException(CouldNotFindByNameMessage("worker pool", missingByNameOnly.ToArray()));
            }
            
            if (WorkerPools is not null && WorkerPools.Any())
            {
                var workerPoolsByNameIdOrSlug =
                    repository.WorkerPools.FindByNameIdOrSlugs<WorkerPoolResource, IWorkerPoolRepository>(WorkerPools, missing => CouldNotFindByMultipleMessage("worker pool", missing.ToArray()));
                workerPools.AddRange(workerPoolsByNameIdOrSlug);
            }
            
            return workerPools;
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
                var workerPoolsByNameIdOrSlug =
                    await repository.WorkerPools.FindByNameIdOrSlugs<WorkerPoolResource, IAsyncWorkerPoolRepository>(
                        WorkerPools, missing => CouldNotFindByMultipleMessage("worker pool", missing.ToArray()));
                workerPools.AddRange(workerPoolsByNameIdOrSlug);
            }
            
            return workerPools;
        }
        
        static void ModifyOrCreateWorker(IOctopusSpaceRepository repository, WorkerResource worker)
        {
            if (IsExistingWorker(worker))
                repository.Workers.Modify(worker);
            else
                repository.Workers.Create(worker);
        }
        
        static async Task ModifyOrCreateWorker(IOctopusSpaceAsyncRepository repository, WorkerResource worker)
        {
            if (IsExistingWorker(worker))
                await repository.Workers.Modify(worker);
            else
                await repository.Workers.Create(worker);
        }
    }
}
