using System;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class WorkerEditor : IResourceEditor<WorkerResource, WorkerEditor>
    {
        private readonly IWorkerRepository repository;

        public WorkerEditor(IWorkerRepository repository)
        {
            this.repository = repository;
        }

        public WorkerResource Instance { get; private set; }

        public async Task<WorkerEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            WorkerPoolResource[] workerpools)
        {
            var existing = await repository.FindByName(name).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new WorkerResource
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

        public WorkerEditor Customize(Action<WorkerResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<WorkerEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}