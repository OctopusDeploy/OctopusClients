using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IWorkerPoolRepository : IFindBySlug<WorkerPoolResource>, IFindByName<WorkerPoolResource>, IGet<WorkerPoolResource>, ICreate<WorkerPoolResource>, IModify<WorkerPoolResource>, IDelete<WorkerPoolResource>, IGetAll<WorkerPoolResource>
    {
        Task<List<WorkerResource>> GetMachines(WorkerPoolResource workerPool,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null);
        Task<WorkerPoolsSummaryResource> Summary(
            string ids = null,
            string partialName = null,
            string machinePartialName = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            bool? hideEmptyPools = false);
        Task Sort(string[] workerPoolIdsInOrder);
        Task<WorkerPoolEditor> CreateOrModify(string name);
        Task<WorkerPoolEditor> CreateOrModify(string name, string description);
    }

    class WorkerPoolRepository : BasicRepository<WorkerPoolResource>, IWorkerPoolRepository
    {
        public WorkerPoolRepository(IOctopusAsyncRepository repository)
            : base(repository, "WorkerPools")
        {
        }

        public async Task<List<WorkerResource>> GetMachines(WorkerPoolResource workerPool,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null)
        {
            var resources = new List<WorkerResource>();

            await Client.Paginate<WorkerResource>(workerPool.Link("Workers"), new {
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
            }).ConfigureAwait(false);

            return resources;
        }

        public async Task<WorkerPoolsSummaryResource> Summary(
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
            }).ConfigureAwait(false);
        }

        public async Task Sort(string[] workerPoolIdsInOrder)
        {
            await Client.Put(await Repository.Link("WorkerPoolSortOrder").ConfigureAwait(false), workerPoolIdsInOrder).ConfigureAwait(false);
        }

        public Task<WorkerPoolEditor> CreateOrModify(string name)
        {
            return new WorkerPoolEditor(this).CreateOrModify(name);
        }

        public Task<WorkerPoolEditor> CreateOrModify(string name, string description)
        {
            return new WorkerPoolEditor(this).CreateOrModify(name, description);
        }
    }
}
