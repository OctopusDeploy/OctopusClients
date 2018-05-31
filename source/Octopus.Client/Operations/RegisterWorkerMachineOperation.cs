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
    public class RegisterWorkerMachineOperation : RegisterMachineOperationBase, IRegisterWorkerMachineOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterWorkerMachineOperation" /> class.
        /// </summary>
        public RegisterWorkerMachineOperation() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterWorkerMachineOperation" /> class.
        /// </summary>
        /// <param name="clientFactory">The client factory.</param>
        public RegisterWorkerMachineOperation(IOctopusClientFactory clientFactory) : base(clientFactory)
        {

        }

        /// <summary>
        /// Gets or sets the worker pools that this machine should be added to.
        /// </summary>
        public string[] WorkerPoolNames { get; set; }

#if SYNC_CLIENT

        /// <summary>
        /// Executes the operation against the specified Octopus Deploy server.
        /// </summary>
        /// <param name="repository">The Octopus Deploy server repository.</param>
        /// <exception cref="System.ArgumentException">
        /// </exception>
        public override void Execute(IOctopusRepository repository)
        {
            var selectedPools = GetWorkerPools(repository);
            var machinePolicy = GetMachinePolicy(repository);
            var machine = GetWorkerMachine(repository);
            var proxy = GetProxy(repository);

            ApplyBaseChanges(machine, machinePolicy, proxy);

            machine.WorkerPoolIds = new ReferenceCollection(selectedPools.Select(p => p.Id).ToArray());

            if (machine.Id != null)
                repository.WorkerMachines.Modify(machine);
            else
                repository.WorkerMachines.Create(machine);
        }

        List<WorkerPoolResource> GetWorkerPools(IOctopusRepository repository)
        {
            var selectedPools = repository.WorkerPools.FindByNames(WorkerPoolNames);

            var missing = WorkerPoolNames.Except(selectedPools.Select(p => p.Name), StringComparer.OrdinalIgnoreCase).ToList();

            if (missing.Any())
                throw new ArgumentException(CouldNotFindMessage("worker pool", missing.ToArray()));

            return selectedPools;
        }

        WorkerMachineResource GetWorkerMachine(IOctopusRepository repository)
        {
            var existing = default(WorkerMachineResource);
            try
            {
                existing = repository.WorkerMachines.FindByName(MachineName);
                if (!AllowOverwrite && existing?.Id != null)
                    throw new ArgumentException($"A worker machine named '{MachineName}' already exists. Use the 'force' parameter if you intended to update the existing machine.");
            }
            catch (OctopusDeserializationException) // eat it, probably caused by resource incompatability between versions
            {
            }
            return existing ?? new WorkerMachineResource();
        }
#endif

        /// <summary>
        /// Executes the operation against the specified Octopus Deploy server.
        /// </summary>
        /// <param name="repository">The Octopus Deploy server repository.</param>
        /// <exception cref="System.ArgumentException">
        /// </exception>
        public override async Task ExecuteAsync(IOctopusAsyncRepository repository)
        {
            var selectedPools = GetWorkerPools(repository).ConfigureAwait(false);
            var machinePolicy = GetMachinePolicy(repository).ConfigureAwait(false);
            var machineTask = GetWorkerMachine(repository).ConfigureAwait(false);
            var proxy = GetProxy(repository).ConfigureAwait(false);

            var machine = await machineTask;
            ApplyBaseChanges(machine, await machinePolicy, await proxy);

            machine.WorkerPoolIds = new ReferenceCollection((await selectedPools).Select(p => p.Id).ToArray());

            if (machine.Id != null)
                await repository.WorkerMachines.Modify(machine).ConfigureAwait(false);
            else
                await repository.WorkerMachines.Create(machine).ConfigureAwait(false);
        }

        async Task<List<WorkerPoolResource>> GetWorkerPools(IOctopusAsyncRepository repository)
        {
            var selectedPools = await repository.WorkerPools.FindByNames(WorkerPoolNames).ConfigureAwait(false);

            var missing = WorkerPoolNames.Except(selectedPools.Select(p => p.Name), StringComparer.OrdinalIgnoreCase).ToList();

            if (missing.Any())
                throw new ArgumentException(CouldNotFindMessage("worker pool", missing.ToArray()));

            return selectedPools;
        }

        async Task<WorkerMachineResource> GetWorkerMachine(IOctopusAsyncRepository repository)
        {
            var existing = default(WorkerMachineResource);
            try
            {
                existing = await repository.WorkerMachines.FindByName(MachineName).ConfigureAwait(false);
                if (!AllowOverwrite && existing?.Id != null)
                    throw new ArgumentException($"A worker machine named '{MachineName}' already exists. Use the 'force' parameter if you intended to update the existing machine.");
            }
            catch (OctopusDeserializationException) // eat it, probably caused by resource incompatability between versions
            {
            }
            return existing ?? new WorkerMachineResource();
        }
    }
}