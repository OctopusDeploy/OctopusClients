using System;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class WorkerMachineEditor : IResourceEditor<WorkerMachineResource, WorkerMachineEditor>
    {
        private readonly IWorkerMachineRepository repository;

        public WorkerMachineEditor(IWorkerMachineRepository repository)
        {
            this.repository = repository;
        }

        public WorkerMachineResource Instance { get; private set; }

        public async Task<WorkerMachineEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            WorkerPoolResource[] workerpools)
        {
            var existing = await repository.FindByName(name).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new WorkerMachineResource
                {
                    Name = name,
                    Endpoint = endpoint,
                    WorkerPoolIds = new ReferenceCollection(workerpools.Select(e => e.Id))
                }).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Endpoint = endpoint;
                existing.WorkerPoolIds.ReplaceAll(workerpools.Select(e => e.Id));

                Instance = await repository.Modify(existing).ConfigureAwait(false);
            }

            return this;
        }

        public WorkerMachineEditor Customize(Action<WorkerMachineResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<WorkerMachineEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}