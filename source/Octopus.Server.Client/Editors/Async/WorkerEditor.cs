using System;
using System.Linq;
using System.Threading;
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

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<WorkerEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            WorkerPoolResource[] workerpools)
            => CreateOrModify(name, endpoint, workerpools, CancellationToken.None);

        public async Task<WorkerEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            WorkerPoolResource[] workerpools,
            CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new WorkerResource
                {
                    Name = name,
                    Endpoint = endpoint,
                    WorkerPoolIds = new ReferenceCollection(workerpools.Select(e => e.Id))
                }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Endpoint = endpoint;
                existing.WorkerPoolIds.ReplaceAll(workerpools.Select(e => e.Id));

                Instance = await repository.Modify(existing, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        public WorkerEditor Customize(Action<WorkerResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<WorkerEditor> Save()
            => Save(CancellationToken.None);

        public async Task<WorkerEditor> Save(CancellationToken cancellationToken)
        {
            Instance = await repository.Modify(Instance, cancellationToken).ConfigureAwait(false);
            return this;
        }
    }
}
