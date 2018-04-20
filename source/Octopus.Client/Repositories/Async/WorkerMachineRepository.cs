using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Repositories.Async
{
    public interface IWorkerMachineRepository : IFindByName<WorkerMachineResource>, IGet<WorkerMachineResource>, ICreate<WorkerMachineResource>, IModify<WorkerMachineResource>, IDelete<WorkerMachineResource>
    {
        Task<WorkerMachineResource> Discover(string host, int port = 10933, DiscoverableEndpointType? discoverableEndpointType = null);
        Task<MachineConnectionStatus> GetConnectionStatus(WorkerMachineResource machine);
        Task<List<WorkerMachineResource>> FindByThumbprint(string thumbprint);

        Task<WorkerMachineEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            WorkerPoolResource[] pools);

        Task<ResourceCollection<WorkerMachineResource>> List(int skip = 0,
            int? take = null,
            string ids = null,
            string name = null,
            string partialName = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null,
            string workerpoolIds = null);
    }

    class WorkerMachineRepository : BasicRepository<WorkerMachineResource>, IWorkerMachineRepository
    {
        public WorkerMachineRepository(IOctopusAsyncClient client) : base(client, "Workers")
        {
        }

        public Task<WorkerMachineResource> Discover(string host, int port = 10933, DiscoverableEndpointType? type = null)
        {
            return Client.Get<WorkerMachineResource>(Client.RootDocument.Link("DiscoverMachine"), new { host, port, type });
        }

        public Task<MachineConnectionStatus> GetConnectionStatus(WorkerMachineResource workerMachine)
        {
            if (workerMachine == null) throw new ArgumentNullException("workerMachine");
            return Client.Get<MachineConnectionStatus>(workerMachine.Link("Connection"));
        }

        public Task<List<WorkerMachineResource>> FindByThumbprint(string thumbprint)
        {
            if (thumbprint == null) throw new ArgumentNullException("thumbprint");
            return Client.Get<List<WorkerMachineResource>>(Client.RootDocument.Link("Workers"), new { id = "all", thumbprint });
        }

        public Task<WorkerMachineEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            WorkerPoolResource[] workerpools)
        {
            return new WorkerMachineEditor(this).CreateOrModify(name, endpoint, workerpools);
        }

        public Task<ResourceCollection<WorkerMachineResource>> List(int skip = 0,
            int? take = null,
            string ids = null,
            string name = null,
            string partialName = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null,
            string workerpoolIds = null)
        {
            return Client.List<WorkerMachineResource>(Client.RootDocument.Link("Workers"), new
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
