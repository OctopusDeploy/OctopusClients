using System.Collections.Generic;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IWorkerPoolRepository : IFindByName<WorkerPoolResource>, IGet<WorkerPoolResource>, ICreate<WorkerPoolResource>, IModify<WorkerPoolResource>, IDelete<WorkerPoolResource>, IGetAll<WorkerPoolResource>
    {
        List<WorkerResource> GetMachines(WorkerPoolResource workerPool,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null);
        WorkerPoolsSummaryResource Summary(
            string ids = null,
            string partialName = null,
            string machinePartialName = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null,
            bool? hideEmptypools = false);
        void Sort(string[] workerpoolIdsInOrder);
        WorkerPoolEditor CreateOrModify(string name);
        WorkerPoolEditor CreateOrModify(string name, string description);
    }

    class WorkerPoolRepository : BasicRepository<WorkerPoolResource>, IWorkerPoolRepository
    {
        public WorkerPoolRepository(IOctopusRepository repository)
            : base(repository, "WorkerPools")
        {
        }

        public List<WorkerResource> GetMachines(WorkerPoolResource workerPool,
            int? skip = 0,
            int? take = null,
            string partialName = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null)
        {
            var resources = new List<WorkerResource>();

            Client.Paginate<WorkerResource>(workerPool.Link("Workers"), new
            {
                skip,
                take,
                partialName,
                isDisabled,
                healthStatuses,
                commStyles
            }, page =>
            {
                resources.AddRange(page.Items);
                return true;
            });

            return resources;
        }

        public WorkerPoolsSummaryResource Summary(
            string ids = null,
            string partialName = null,
            string machinePartialName = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null,
            bool? hideEmptyPools = false)
        {
            return Client.Get<WorkerPoolsSummaryResource>(Repository.Link("WorkerPoolsSummary"), new
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

        public void Sort(string[] workerpoolIdsInOrder)
        {
            Client.Put(Repository.Link("WorkerPoolsSortOrder"), workerpoolIdsInOrder);
        }

        public WorkerPoolEditor CreateOrModify(string name)
        {
            return new WorkerPoolEditor(this).CreateOrModify(name);
        }

        public WorkerPoolEditor CreateOrModify(string name, string description)
        {
            return new WorkerPoolEditor(this).CreateOrModify(name, description);
        }
    }
}