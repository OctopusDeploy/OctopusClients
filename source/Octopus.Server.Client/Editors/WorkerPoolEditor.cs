using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class WorkerPoolEditor : IResourceEditor<WorkerPoolResource, WorkerPoolEditor>
    {
        private readonly IWorkerPoolRepository repository;

        public WorkerPoolEditor(IWorkerPoolRepository repository)
        {
            this.repository = repository;
        }

        public WorkerPoolResource Instance { get; private set; }

        public WorkerPoolEditor CreateOrModify(string name)
        {
            var existing = repository.FindByName(name);
            if (existing == null)
            {
                Instance = repository.Create(new WorkerPoolResource
                {
                    Name = name,
                });
            }
            else
            {
                existing.Name = name;

                Instance = repository.Modify(existing);
            }

            return this;
        }

        public WorkerPoolEditor CreateOrModify(string name, string description)
        {
            var existing = repository.FindByName(name);
            if (existing == null)
            {
                Instance = repository.Create(new WorkerPoolResource
                {
                    Name = name,
                    Description = description
                });
            }
            else
            {
                existing.Name = name;
                existing.Description = description;

                Instance = repository.Modify(existing);
            }

            return this;
        }

        public WorkerPoolEditor Customize(Action<WorkerPoolResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public WorkerPoolEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}