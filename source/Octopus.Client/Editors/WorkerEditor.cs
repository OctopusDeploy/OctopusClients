using System;
using System.Linq;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class WorkerEditor : IResourceEditor<WorkerResource, WorkerEditor>
    {
        private readonly IWorkerRepository repository;

        public WorkerEditor(IWorkerRepository repository)
        {
            this.repository = repository;
        }

        public WorkerResource Instance { get; private set; }

        public WorkerEditor CreateOrModify(
            string name,
            EndpointResource endpoint,
            WorkerPoolResource[] pools)
        {
            var existing = repository.FindByName(name);
            if (existing == null)
            {
                Instance = repository.Create(new WorkerResource
                {
                    Name = name,
                    Endpoint = endpoint,
                    WorkerPoolIds = new ReferenceCollection(pools.Select(e => e.Id))
                });
            }
            else
            {
                existing.Name = name;
                existing.Endpoint = endpoint;
                existing.WorkerPoolIds.ReplaceAll(pools.Select(e => e.Id));

                Instance = repository.Modify(existing);
            }

            return this;
        }

        public WorkerEditor Customize(Action<WorkerResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public WorkerEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}