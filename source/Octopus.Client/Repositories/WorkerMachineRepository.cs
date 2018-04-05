using System;
using System.Collections.Generic;
using Octopus.Client.Editors;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Repositories
{
    public interface IWorkerMachineRepository : IFindByName<WorkerMachineResource>, IGet<WorkerMachineResource>, ICreate<WorkerMachineResource>, IModify<WorkerMachineResource>, IDelete<WorkerMachineResource>
    {
        WorkerMachineResource Discover(string host, int port = 10933, DiscoverableEndpointType? discoverableEndpointType = null);
        MachineConnectionStatus GetConnectionStatus(WorkerMachineResource machine);
        List<WorkerMachineResource> FindByThumbprint(string thumbprint);

        WorkerMachineEditor CreateOrModify(
            string name,
            EndpointResource endpoint,
            WorkerPoolResource[] pools);

        ResourceCollection<WorkerMachineResource> List(int skip = 0,
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
        public WorkerMachineRepository(IOctopusClient client) : base(client, "Workers")
        {
        }

        public WorkerMachineResource Discover(string host, int port = 10933, DiscoverableEndpointType? type = null)
        {
            return Client.Get<WorkerMachineResource>(Client.RootDocument.Link("DiscoverMachine"), new { host, port, type });
        }

        public MachineConnectionStatus GetConnectionStatus(WorkerMachineResource machine)
        {
            if (machine == null) throw new ArgumentNullException("machine");
            return Client.Get<MachineConnectionStatus>(machine.Link("Connection"));
        }

        public List<WorkerMachineResource> FindByThumbprint(string thumbprint)
        {
            if (thumbprint == null) throw new ArgumentNullException("thumbprint");
            return Client.Get<List<WorkerMachineResource>>(Client.RootDocument.Link("workers"), new { id = "all", thumbprint });
        }


        public WorkerMachineEditor CreateOrModify(
            string name,
            EndpointResource endpoint,
            WorkerPoolResource[] pools)
        {
            return new WorkerMachineEditor(this).CreateOrModify(name, endpoint, pools);
        }

        public ResourceCollection<WorkerMachineResource> List(int skip = 0,
            int? take = null,
            string ids = null,
            string name = null,
            string partialName = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null,
            string workerpoolIds = null)
        {
            return Client.List<WorkerMachineResource>(Client.RootDocument.Link("workers"), new
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