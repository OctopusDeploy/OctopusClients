using System;
using System.Threading;
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

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<WorkerPoolEditor> CreateOrModify(string name)
            => CreateOrModify(name, CancellationToken.None);

        public async Task<WorkerPoolEditor> CreateOrModify(string name, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new WorkerPoolResource
                {
                    Name = name,
                }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;

                Instance = await repository.Modify(existing, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<WorkerPoolEditor> CreateOrModify(string name, string description)
            => CreateOrModify(name, description, CancellationToken.None);

        public async Task<WorkerPoolEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new WorkerPoolResource
                {
                    Name = name,
                    Description = description
                }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Description = description;

                Instance = await repository.Modify(existing, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        public WorkerPoolEditor Customize(Action<WorkerPoolResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<WorkerPoolEditor> Save()
            => Save(CancellationToken.None);

        public async Task<WorkerPoolEditor> Save(CancellationToken cancellationToken)
        {
            Instance = await repository.Modify(Instance, cancellationToken).ConfigureAwait(false);
            return this;
        }
    }
}
