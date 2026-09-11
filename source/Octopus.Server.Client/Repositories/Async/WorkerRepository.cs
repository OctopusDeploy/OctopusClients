using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Repositories.Async
{
    public interface IWorkerRepository : IFindByName<WorkerResource>, IGet<WorkerResource>, ICreate<WorkerResource>, IModify<WorkerResource>, IDelete<WorkerResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<WorkerResource> Discover(string host, int port = 10933, DiscoverableEndpointType? discoverableEndpointType = null);
        Task<WorkerResource> Discover(string host, CancellationToken cancellationToken, int port = 10933, DiscoverableEndpointType? discoverableEndpointType = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<MachineConnectionStatus> GetConnectionStatus(WorkerResource machine);
        Task<MachineConnectionStatus> GetConnectionStatus(WorkerResource machine, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<WorkerResource>> FindByThumbprint(string thumbprint);
        Task<List<WorkerResource>> FindByThumbprint(string thumbprint, CancellationToken cancellationToken);

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<WorkerEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            WorkerPoolResource[] pools);
        Task<WorkerEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            WorkerPoolResource[] pools,
            CancellationToken cancellationToken);

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<WorkerResource>> List(int skip = 0,
            int? take = null,
            string ids = null,
            string name = null,
            string partialName = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            string workerpoolIds = null);
        Task<ResourceCollection<WorkerResource>> List(CancellationToken cancellationToken,
            int skip = 0,
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
        public WorkerRepository(IOctopusAsyncRepository repository) : base(repository, "Workers")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<WorkerResource> Discover(string host, int port = 10933, DiscoverableEndpointType? type = null)
            => Discover(host, CancellationToken.None, port, type);

        public async Task<WorkerResource> Discover(string host, CancellationToken cancellationToken, int port = 10933, DiscoverableEndpointType? type = null)
        {
            return await Client.Get<WorkerResource>(await Repository.Link("DiscoverWorker").ConfigureAwait(false), new { host, port, type }, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<MachineConnectionStatus> GetConnectionStatus(WorkerResource worker)
            => GetConnectionStatus(worker, CancellationToken.None);

        public Task<MachineConnectionStatus> GetConnectionStatus(WorkerResource worker, CancellationToken cancellationToken)
        {
            if (worker == null) throw new ArgumentNullException("worker");
            return Client.Get<MachineConnectionStatus>(worker.Link("Connection"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<WorkerResource>> FindByThumbprint(string thumbprint)
            => FindByThumbprint(thumbprint, CancellationToken.None);

        public async Task<List<WorkerResource>> FindByThumbprint(string thumbprint, CancellationToken cancellationToken)
        {
            if (thumbprint == null) throw new ArgumentNullException("thumbprint");
            return await Client.Get<List<WorkerResource>>(await Repository.Link("Workers").ConfigureAwait(false), new { id = IdValueConstant.IdAll, thumbprint }, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<WorkerEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            WorkerPoolResource[] workerpools)
            => CreateOrModify(name, endpoint, workerpools, CancellationToken.None);

        public Task<WorkerEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            WorkerPoolResource[] workerpools,
            CancellationToken cancellationToken)
        {
            return new WorkerEditor(this).CreateOrModify(name, endpoint, workerpools);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ResourceCollection<WorkerResource>> List(int skip = 0,
            int? take = null,
            string ids = null,
            string name = null,
            string partialName = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            string workerpoolIds = null)
            => List(CancellationToken.None, skip, take, ids, name, partialName, isDisabled, healthStatuses, commStyles, workerpoolIds);

        public async Task<ResourceCollection<WorkerResource>> List(CancellationToken cancellationToken,
            int skip = 0,
            int? take = null,
            string ids = null,
            string name = null,
            string partialName = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            string workerpoolIds = null)
        {
            return await Client.List<WorkerResource>(await Repository.Link("Workers").ConfigureAwait(false), new
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
            }, cancellationToken).ConfigureAwait(false);
        }
    }
}
