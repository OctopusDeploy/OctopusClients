using System;
using System.Linq;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class WorkerMachineEditor : IResourceEditor<WorkerMachineResource, WorkerMachineEditor>
    {
        private readonly IWorkerMachineRepository repository;

        public WorkerMachineEditor(IWorkerMachineRepository repository)
        {
            this.repository = repository;
        }

        public WorkerMachineResource Instance { get; private set; }

        public WorkerMachineEditor CreateOrModify(
            string name,
            EndpointResource endpoint,
            WorkerPoolResource[] pools)
        {
            var existing = repository.FindByName(name);
            if (existing == null)
            {
                Instance = repository.Create(new WorkerMachineResource
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

        public WorkerMachineEditor Customize(Action<WorkerMachineResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public WorkerMachineEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}