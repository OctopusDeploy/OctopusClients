using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class WorkerPoolEditor : IResourceEditor<WorkerPoolResource, WorkerPoolEditor>
    {
        private readonly IWorkerPoolRepository repository;

        public WorkerPoolEditor(IWorkerPoolRepository repository)
        {
            this.repository = repository;
        }

        public WorkerPoolResource Instance { get; private set; }

        public async Task<WorkerPoolEditor> CreateOrModify(string name)
        {
            var existing = await repository.FindByName(name).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new WorkerPoolResource
                {
                    Name = name,
                }).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;

                Instance = await repository.Modify(existing).ConfigureAwait(false);
            }

            return this;
        }

        public async Task<WorkerPoolEditor> CreateOrModify(string name, string description)
        {
            var existing = await repository.FindByName(name).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new WorkerPoolResource
                {
                    Name = name,
                    Description = description
                }).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Description = description;

                Instance = await repository.Modify(existing).ConfigureAwait(false);
            }

            return this;
        }

        public WorkerPoolEditor Customize(Action<WorkerPoolResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<WorkerPoolEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}