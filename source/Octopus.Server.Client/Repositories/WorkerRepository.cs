using System;
using System.Collections.Generic;
using Octopus.Client.Editors;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Repositories
{
    public interface IWorkerRepository : IFindByName<WorkerResource>, IGet<WorkerResource>, ICreate<WorkerResource>, IModify<WorkerResource>, IDelete<WorkerResource>
    {
        WorkerResource Discover(string host, int port = 10933, DiscoverableEndpointType? discoverableEndpointType = null);
        MachineConnectionStatus GetConnectionStatus(WorkerResource machine);
        List<WorkerResource> FindByThumbprint(string thumbprint);

        WorkerEditor CreateOrModify(
            string name,
            EndpointResource endpoint,
            WorkerPoolResource[] pools);

        ResourceCollection<WorkerResource> List(int skip = 0,
            int? take = null,
            string ids = null,
            string name = null,
            string partialName = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            string workerpoolIds = null);
    }

    class WorkerRepository : BasicRepository<WorkerResource>, IWorkerRepository
    {
        public WorkerRepository(IOctopusRepository repository) : base(repository, "Workers")
        {
        }

        public WorkerResource Discover(string host, int port = 10933, DiscoverableEndpointType? type = null)
        {
            return Client.Get<WorkerResource>(Repository.Link("DiscoverWorker"), new { host, port, type });
        }

        public MachineConnectionStatus GetConnectionStatus(WorkerResource machine)
        {
            if (machine == null) throw new ArgumentNullException("machine");
            return Client.Get<MachineConnectionStatus>(machine.Link("Connection"));
        }

        public List<WorkerResource> FindByThumbprint(string thumbprint)
        {
            if (thumbprint == null) throw new ArgumentNullException("thumbprint");
            return Client.Get<List<WorkerResource>>(Repository.Link("Workers"), new { id = IdValueConstant.IdAll, thumbprint });
        }


        public WorkerEditor CreateOrModify(
            string name,
            EndpointResource endpoint,
            WorkerPoolResource[] pools)
        {
            return new WorkerEditor(this).CreateOrModify(name, endpoint, pools);
        }

        public ResourceCollection<WorkerResource> List(int skip = 0,
            int? take = null,
            string ids = null,
            string name = null,
            string partialName = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            string workerpoolIds = null)
        {
            return Client.List<WorkerResource>(Repository.Link("workers"), new
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