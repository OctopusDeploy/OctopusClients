using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Repositories.Async
{
    public interface IWorkerRepository : IFindByName<WorkerResource>, IGet<WorkerResource>, ICreate<WorkerResource>, IModify<WorkerResource>, IDelete<WorkerResource>
    {
        Task<WorkerResource> Discover(string host, int port = 10933, DiscoverableEndpointType? discoverableEndpointType = null);
        Task<MachineConnectionStatus> GetConnectionStatus(WorkerResource machine);
        Task<List<WorkerResource>> FindByThumbprint(string thumbprint);

        Task<WorkerEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            WorkerPoolResource[] pools);

        Task<ResourceCollection<WorkerResource>> List(int skip = 0,
            int? take = null,
            string ids = null,
            string name = null,
            string partialName = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null,
            string workerpoolIds = null);
    }

    class WorkerRepository : BasicRepository<WorkerResource>, IWorkerRepository
    {
        public WorkerRepository(IOctopusAsyncRepository repository) : base(repository, "Workers")
        {
        }

        public Task<WorkerResource> Discover(string host, int port = 10933, DiscoverableEndpointType? type = null)
        {
            return Client.Get<WorkerResource>(Repository.Link("DiscoverMachine"), new { host, port, type });
        }

        public Task<MachineConnectionStatus> GetConnectionStatus(WorkerResource worker)
        {
            if (worker == null) throw new ArgumentNullException("worker");
            return Client.Get<MachineConnectionStatus>(worker.Link("Connection"));
        }

        public Task<List<WorkerResource>> FindByThumbprint(string thumbprint)
        {
            if (thumbprint == null) throw new ArgumentNullException("thumbprint");
            return Client.Get<List<WorkerResource>>(Repository.Link("Workers"), new { id = IdValueConstant.IdAll, thumbprint });
        }

        public Task<WorkerEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            WorkerPoolResource[] workerpools)
        {
            return new WorkerEditor(this).CreateOrModify(name, endpoint, workerpools);
        }

        public Task<ResourceCollection<WorkerResource>> List(int skip = 0,
            int? take = null,
            string ids = null,
            string name = null,
            string partialName = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null,
            string workerpoolIds = null)
        {
            return Client.List<WorkerResource>(Repository.Link("Workers"), new
            {
                skip,
                take,
                ids,
                name,
                partialName,
                isDisabled,
                healthStatuses,
                commStyles,
                workerpoolIds
            });
        }
    }
}
