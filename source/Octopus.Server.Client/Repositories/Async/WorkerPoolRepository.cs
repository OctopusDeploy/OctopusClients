using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IWorkerPoolRepository : IFindBySlug<WorkerPoolResource>, IFindByName<WorkerPoolResource>, IGet<WorkerPoolResource>, ICreate<WorkerPoolResource>, IModify<WorkerPoolResource>, IDelete<WorkerPoolResource>, IGetAll<WorkerPoolResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<WorkerResource>> GetMachines(WorkerPoolResource workerPool,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null);
        Task<List<WorkerResource>> GetMachines(WorkerPoolResource workerPool,
            CancellationToken cancellationToken,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<WorkerPoolsSummaryResource> Summary(
            string ids = null,
            string partialName = null,
            string machinePartialName = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            bool? hideEmptyPools = false);
        Task<WorkerPoolsSummaryResource> Summary(
            CancellationToken cancellationToken,
            string ids = null,
            string partialName = null,
            string machinePartialName = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            bool? hideEmptyPools = false);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task Sort(string[] workerPoolIdsInOrder);
        Task Sort(string[] workerPoolIdsInOrder, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<WorkerPoolEditor> CreateOrModify(string name);
        Task<WorkerPoolEditor> CreateOrModify(string name, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<WorkerPoolEditor> CreateOrModify(string name, string description);
        Task<WorkerPoolEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken);
    }

    class WorkerPoolRepository : BasicRepository<WorkerPoolResource>, IWorkerPoolRepository
    {
        public WorkerPoolRepository(IOctopusAsyncRepository repository)
            : base(repository, "WorkerPools")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<WorkerResource>> GetMachines(WorkerPoolResource workerPool,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null)
            => GetMachines(workerPool, CancellationToken.None, skip, take, partialName, isDisabled, healthStatuses, commStyles);

        public async Task<List<WorkerResource>> GetMachines(WorkerPoolResource workerPool,
            CancellationToken cancellationToken,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null)
        {
            var resources = new List<WorkerResource>();

            await Client.Paginate<WorkerResource>(workerPool.Link("Workers"), new
            {
                skip,
                take,
                partialName,
                isDisabled,
                healthStatuses,
                commStyles,
            }, page =>
            {
                resources.AddRange(page.Items);
                return true;
            }, cancellationToken).ConfigureAwait(false);

            return resources;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<WorkerPoolsSummaryResource> Summary(
            string ids = null,
            string partialName = null,
            string machinePartialName = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            bool? hideEmptyPools = false)
            => Summary(CancellationToken.None, ids, partialName, machinePartialName, isDisabled, healthStatuses, commStyles, hideEmptyPools);

        public async Task<WorkerPoolsSummaryResource> Summary(
            CancellationToken cancellationToken,
            string ids = null,
            string partialName = null,
            string machinePartialName = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            bool? hideEmptyPools = false)
        {
            return await Client.Get<WorkerPoolsSummaryResource>(await Repository.Link("WorkerPoolsSummary").ConfigureAwait(false), new
            {
                ids,
                partialName,
                machinePartialName,
                isDisabled,
                healthStatuses,
                commStyles,
                hideEmptyPools,
            }, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task Sort(string[] workerPoolIdsInOrder)
            => Sort(workerPoolIdsInOrder, CancellationToken.None);

        public async Task Sort(string[] workerPoolIdsInOrder, CancellationToken cancellationToken)
        {
            await Client.Put(await Repository.Link("WorkerPoolSortOrder").ConfigureAwait(false), workerPoolIdsInOrder, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<WorkerPoolEditor> CreateOrModify(string name)
            => CreateOrModify(name, CancellationToken.None);

        public Task<WorkerPoolEditor> CreateOrModify(string name, CancellationToken cancellationToken)
        {
            return new WorkerPoolEditor(this).CreateOrModify(name);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<WorkerPoolEditor> CreateOrModify(string name, string description)
            => CreateOrModify(name, description, CancellationToken.None);

        public Task<WorkerPoolEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken)
        {
            return new WorkerPoolEditor(this).CreateOrModify(name, description);
        }
    }
}
