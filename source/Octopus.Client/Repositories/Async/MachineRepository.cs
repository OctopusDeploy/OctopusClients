using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Repositories.Async
{
    public interface IMachineRepository : IFindByName<DeploymentTargetResource>, IGet<DeploymentTargetResource>, ICreate<DeploymentTargetResource>, IModify<DeploymentTargetResource>, IDelete<DeploymentTargetResource>
    {
        Task<DeploymentTargetResource> Discover(string host, int port = 10933, DiscoverableEndpointType? discoverableEndpointType = null);
        Task<MachineConnectionStatus> GetConnectionStatus(DeploymentTargetResource deploymentTarget);
        Task<List<DeploymentTargetResource>> FindByThumbprint(string thumbprint);
        Task<IReadOnlyList<TaskResource>> GetTasks(DeploymentTargetResource deploymentTarget);
        Task<IReadOnlyList<TaskResource>> GetTasks(DeploymentTargetResource deploymentTarget, object pathParameters);

        Task<DeploymentTargetEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles,
            TenantResource[] tenants,
            TagResource[] tenantTags,
            TenantedDeploymentMode? tenantedDeploymentParticipation);

        Task<DeploymentTargetEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles);

        Task<ResourceCollection<DeploymentTargetResource>> List(int skip = 0,
            int? take = null,
            string ids = null,
            string name = null,
            string partialName = null,
            string roles = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null,
            string environmentIds = null);
    }

    class MachineRepository : BasicRepository<DeploymentTargetResource>, IMachineRepository
    {
        public MachineRepository(IOctopusAsyncClient client) : base(client, "Machines")
        {
        }

        public Task<DeploymentTargetResource> Discover(string host, int port = 10933, DiscoverableEndpointType? type = null)
        {
            return Client.Get<DeploymentTargetResource>(Client.RootDocument.Link("DiscoverMachine"), new { host, port, type });
        }

        public Task<MachineConnectionStatus> GetConnectionStatus(DeploymentTargetResource deploymentTarget)
        {
            if (deploymentTarget == null) throw new ArgumentNullException("deploymentTarget");
            return Client.Get<MachineConnectionStatus>(deploymentTarget.Link("Connection"));
        }

        public Task<List<DeploymentTargetResource>> FindByThumbprint(string thumbprint)
        {
            if (thumbprint == null) throw new ArgumentNullException("thumbprint");
            return Client.Get<List<DeploymentTargetResource>>(Client.RootDocument.Link("machines"), new { id = "all", thumbprint });
        }

        /// <summary>
        /// Gets all tasks involving the specified machine
        /// </summary>
        /// <param name="deploymentTarget"></param>
        /// <returns></returns>
        public Task<IReadOnlyList<TaskResource>> GetTasks(DeploymentTargetResource deploymentTarget) => GetTasks(deploymentTarget, new {  skip = 0 });

        /// <summary>
        /// Gets all tasks associated with this machine
        ///
        /// The `take` pathParmeter is only respected in Octopus 4.0.6 and later
        /// </summary>
        /// <param name="deploymentTarget"></param>
        /// <param name="pathParameters"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<TaskResource>> GetTasks(DeploymentTargetResource deploymentTarget, object pathParameters)
        {
            if (deploymentTarget == null)
                throw new ArgumentNullException(nameof(deploymentTarget));

            return await Client.ListAll<TaskResource>(deploymentTarget.Link("TasksTemplate"), pathParameters).ConfigureAwait(false);
        }

        public Task<DeploymentTargetEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles,
            TenantResource[] tenants,
            TagResource[] tenantTags,
            TenantedDeploymentMode? tenantedDeploymentParticipation)
        {
            return new DeploymentTargetEditor(this).CreateOrModify(name, endpoint, environments, roles, tenants, tenantTags, tenantedDeploymentParticipation);
        }

        public Task<DeploymentTargetEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles)
        {
            return new DeploymentTargetEditor(this).CreateOrModify(name, endpoint, environments, roles);
        }

        public Task<ResourceCollection<DeploymentTargetResource>> List(int skip = 0,
            int? take = null,
            string ids = null,
            string name = null,
            string partialName = null,
            string roles = null,
            bool? isDisabled = false,
            string healthStatuses = null,
            string commStyles = null,
            string tenantIds = null,
            string tenantTags = null,
            string environmentIds = null)
        {
            return Client.List<DeploymentTargetResource>(Client.RootDocument.Link("Machines"), new
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
            });
        }
    }
}
