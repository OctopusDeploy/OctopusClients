using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IWorkerPoolRepository : IFindByName<WorkerPoolResource>, IGet<WorkerPoolResource>, ICreate<WorkerPoolResource>, IModify<WorkerPoolResource>, IDelete<WorkerPoolResource>, IGetAll<WorkerPoolResource>
    {
        Task<List<WorkerMachineResource>> GetMachines(WorkerPoolResource pool,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null);
        Task<WorkerPoolsSummaryResource> Summary(
            string ids = null,
            string partialName = null,
            string machinePartialName = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null,
            bool? hideEmptyPools = false);
        Task Sort(string[] workerpoolIdsInOrder);
        Task<WorkerPoolEditor> CreateOrModify(string name);
        Task<WorkerPoolEditor> CreateOrModify(string name, string description);
    }

    class WorkerPoolRepository : BasicRepository<WorkerPoolResource>, IWorkerPoolRepository
    {
        public WorkerPoolRepository(IOctopusAsyncClient client)
            : base(client, "WorkerPools")
        {
        }

        public async Task<List<WorkerMachineResource>> GetMachines(WorkerPoolResource environment,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null)
        {
            var resources = new List<WorkerMachineResource>();

            await Client.Paginate<WorkerMachineResource>(environment.Link("WorkerMachines"), new {
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

        public Task<WorkerPoolsSummaryResource> Summary(
            string ids = null,
            string partialName = null,
            string machinePartialName = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null,
            bool? hideEmptyPools = false)
        {
            return Client.Get<WorkerPoolsSummaryResource>(Client.RootDocument.Link("WorkerPoolsSummary"), new
            {
                ids,
                partialName,
                machinePartialName,
                isDisabled,
                healthStatuses,
                commStyles,
                hideEmptyPools,
            });
        }

        public Task Sort(string[] environmentIdsInOrder)
        {
            return Client.Put(Client.RootDocument.Link("WorkerPoolSortOrder"), environmentIdsInOrder);
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
