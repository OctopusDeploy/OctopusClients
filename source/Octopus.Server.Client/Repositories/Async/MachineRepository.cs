using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Repositories.Async
{
    public interface IMachineRepository : IFindByName<MachineResource>, IGet<MachineResource>, ICreate<MachineResource>, IModify<MachineResource>, IDelete<MachineResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<MachineResource> Discover(string host, int port = 10933, DiscoverableEndpointType? discoverableEndpointType = null);
        Task<MachineResource> Discover(string host, CancellationToken cancellationToken, int port = 10933, DiscoverableEndpointType? discoverableEndpointType = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<MachineResource> Discover(DiscoverMachineOptions options);
        Task<MachineResource> Discover(DiscoverMachineOptions options, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<MachineConnectionStatus> GetConnectionStatus(MachineResource machine);
        Task<MachineConnectionStatus> GetConnectionStatus(MachineResource machine, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<MachineResource>> FindByThumbprint(string thumbprint);
        Task<List<MachineResource>> FindByThumbprint(string thumbprint, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<IReadOnlyList<TaskResource>> GetTasks(MachineResource machine);
        Task<IReadOnlyList<TaskResource>> GetTasks(MachineResource machine, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<IReadOnlyList<TaskResource>> GetTasks(MachineResource machine, object pathParameters);
        Task<IReadOnlyList<TaskResource>> GetTasks(MachineResource machine, object pathParameters, CancellationToken cancellationToken);

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<MachineEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles,
            TenantResource[] tenants,
            TagResource[] tenantTags,
            TenantedDeploymentMode? tenantedDeploymentParticipation);
        Task<MachineEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles,
            TenantResource[] tenants,
            TagResource[] tenantTags,
            TenantedDeploymentMode? tenantedDeploymentParticipation,
            CancellationToken cancellationToken);

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<MachineEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles);
        Task<MachineEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles,
            CancellationToken cancellationToken);

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<MachineResource>> List(int skip = 0,
            int? take = null,
            string ids = null,
            string name = null,
            string partialName = null,
            string roles = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null,
            string environmentIds = null);
        Task<ResourceCollection<MachineResource>> List(CancellationToken cancellationToken,
            int skip = 0,
            int? take = null,
            string ids = null,
            string name = null,
            string partialName = null,
            string roles = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null,
            string environmentIds = null);
    }

    class MachineRepository : BasicRepository<MachineResource>, IMachineRepository
    {
        public MachineRepository(IOctopusAsyncRepository repository) : base(repository, "Machines")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<MachineResource> Discover(string host, int port = 10933, DiscoverableEndpointType? type = null)
            => Discover(host, CancellationToken.None, port, type);

        public Task<MachineResource> Discover(string host, CancellationToken cancellationToken, int port = 10933, DiscoverableEndpointType? type = null)
            => Discover(new DiscoverMachineOptions(host)
            {
                Port = port,
                Type = type
            }, cancellationToken);

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<MachineResource> Discover(DiscoverMachineOptions options)
            => Discover(options, CancellationToken.None);

        public async Task<MachineResource> Discover(DiscoverMachineOptions options, CancellationToken cancellationToken)
            => await Client.Get<MachineResource>(await Repository.Link("DiscoverMachine").ConfigureAwait(false), new
            {
                host = options.Host,
                port = options.Port,
                type = options.Type,
                proxyId = options.Proxy?.Id
            }, cancellationToken).ConfigureAwait(false);

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<MachineConnectionStatus> GetConnectionStatus(MachineResource machine)
            => GetConnectionStatus(machine, CancellationToken.None);

        public Task<MachineConnectionStatus> GetConnectionStatus(MachineResource machine, CancellationToken cancellationToken)
        {
            if (machine == null) throw new ArgumentNullException("machine");
            return Client.Get<MachineConnectionStatus>(machine.Link("Connection"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<MachineResource>> FindByThumbprint(string thumbprint)
            => FindByThumbprint(thumbprint, CancellationToken.None);

        public async Task<List<MachineResource>> FindByThumbprint(string thumbprint, CancellationToken cancellationToken)
        {
            if (thumbprint == null) throw new ArgumentNullException("thumbprint");
            return await Client.Get<List<MachineResource>>(await Repository.Link("machines").ConfigureAwait(false), new { id = IdValueConstant.IdAll, thumbprint }, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all tasks involving the specified machine
        /// </summary>
        /// <param name="machine"></param>
        /// <returns></returns>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<IReadOnlyList<TaskResource>> GetTasks(MachineResource machine) => GetTasks(machine, CancellationToken.None);

        public Task<IReadOnlyList<TaskResource>> GetTasks(MachineResource machine, CancellationToken cancellationToken) => GetTasks(machine, new { skip = 0 }, cancellationToken);

        /// <summary>
        /// Gets all tasks associated with this machine
        ///
        /// The `take` pathParmeter is only respected in Octopus 4.0.6 and later
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="pathParameters"></param>
        /// <returns></returns>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<IReadOnlyList<TaskResource>> GetTasks(MachineResource machine, object pathParameters)
            => GetTasks(machine, pathParameters, CancellationToken.None);

        public async Task<IReadOnlyList<TaskResource>> GetTasks(MachineResource machine, object pathParameters, CancellationToken cancellationToken)
        {
            if (machine == null)
                throw new ArgumentNullException(nameof(machine));

            return await Client.ListAll<TaskResource>(machine.Link("TasksTemplate"), pathParameters, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<MachineEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles,
            TenantResource[] tenants,
            TagResource[] tenantTags,
            TenantedDeploymentMode? tenantedDeploymentParticipation)
            => CreateOrModify(name, endpoint, environments, roles, tenants, tenantTags, tenantedDeploymentParticipation, CancellationToken.None);

        public Task<MachineEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles,
            TenantResource[] tenants,
            TagResource[] tenantTags,
            TenantedDeploymentMode? tenantedDeploymentParticipation,
            CancellationToken cancellationToken)
        {
            return new MachineEditor(this).CreateOrModify(name, endpoint, environments, roles, tenants, tenantTags, tenantedDeploymentParticipation);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<MachineEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles)
            => CreateOrModify(name, endpoint, environments, roles, CancellationToken.None);

        public Task<MachineEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles,
            CancellationToken cancellationToken)
        {
            return new MachineEditor(this).CreateOrModify(name, endpoint, environments, roles);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ResourceCollection<MachineResource>> List(int skip = 0,
            int? take = null,
            string ids = null,
            string name = null,
            string partialName = null,
            string roles = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null,
            string environmentIds = null)
            => List(CancellationToken.None, skip, take, ids, name, partialName, roles, isDisabled, healthStatuses, commStyles, tenantIds, tenantTags, environmentIds);

        public async Task<ResourceCollection<MachineResource>> List(CancellationToken cancellationToken,
            int skip = 0,
            int? take = null,
            string ids = null,
            string name = null,
            string partialName = null,
            string roles = null,
            bool? isDisabled = null,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null,
            string environmentIds = null)
        {
            return await Client.List<MachineResource>(await Repository.Link("Machines").ConfigureAwait(false), new
            {
                skip,
                take,
                ids,
                name,
                partialName,
                roles,
                isDisabled,
                healthStatuses,
                commStyles,
                tenantIds,
                tenantTags,
                environmentIds,
            }, cancellationToken).ConfigureAwait(false);
        }
    }
}
