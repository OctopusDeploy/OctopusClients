using System;
using System.Collections.Generic;
using Octopus.Client.Editors;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Repositories
{
    public interface IMachineRepository : IFindByName<DeploymentTargetResource>, IGet<DeploymentTargetResource>, ICreate<DeploymentTargetResource>, IModify<DeploymentTargetResource>, IDelete<DeploymentTargetResource>
    {
        DeploymentTargetResource Discover(string host, int port = 10933, DiscoverableEndpointType? discoverableEndpointType = null);
        MachineConnectionStatus GetConnectionStatus(DeploymentTargetResource deploymentTarget);
        List<DeploymentTargetResource> FindByThumbprint(string thumbprint);
        IReadOnlyList<TaskResource> GetTasks(DeploymentTargetResource deploymentTarget);
        IReadOnlyList<TaskResource> GetTasks(DeploymentTargetResource deploymentTarget, object pathParameters);


        DeploymentTargetEditor CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles,
            TenantResource[] tenants,
            TagResource[] tenantTags,
            TenantedDeploymentMode? tenantedDeploymentParticipation);

        DeploymentTargetEditor CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles);

        ResourceCollection<DeploymentTargetResource> List(int skip = 0,
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
        public MachineRepository(IOctopusClient client) : base(client, "Machines")
        {
        }

        public DeploymentTargetResource Discover(string host, int port = 10933, DiscoverableEndpointType? type = null)
        {
            return Client.Get<DeploymentTargetResource>(Client.RootDocument.Link("DiscoverMachine"), new { host, port, type });
        }

        public MachineConnectionStatus GetConnectionStatus(DeploymentTargetResource deploymentTarget)
        {
            if (deploymentTarget == null) throw new ArgumentNullException("deploymentTarget");
            return Client.Get<MachineConnectionStatus>(deploymentTarget.Link("Connection"));
        }

        public List<DeploymentTargetResource> FindByThumbprint(string thumbprint)
        {
            if (thumbprint == null) throw new ArgumentNullException("thumbprint");
            return Client.Get<List<DeploymentTargetResource>>(Client.RootDocument.Link("machines"), new { id = "all", thumbprint });
        }

        /// <summary>
        /// Gets all tasks involving the specified machine
        /// </summary>
        /// <param name="deploymentTarget"></param>
        /// <returns></returns>
        public IReadOnlyList<TaskResource> GetTasks(DeploymentTargetResource deploymentTarget) => GetTasks(deploymentTarget, new {skip = 0});

        /// <summary>
        /// Gets all tasks involving the specified machine
        ///
        /// The `take` pathParmeter is only respected in Octopus 4.0.6 and later
        /// </summary>
        /// <param name="deploymentTarget"></param>
        /// <param name="pathParameters"></param>
        /// <returns></returns>
        public IReadOnlyList<TaskResource> GetTasks(DeploymentTargetResource deploymentTarget, object pathParameters)
        {
            if (deploymentTarget == null)
                throw new ArgumentNullException(nameof(deploymentTarget));

            return Client.ListAll<TaskResource>(deploymentTarget.Link("TasksTemplate"), pathParameters);
        }

        public DeploymentTargetEditor CreateOrModify(
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

        public DeploymentTargetEditor CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles)
        {
            return new DeploymentTargetEditor(this).CreateOrModify(name, endpoint, environments, roles);
        }

        public ResourceCollection<DeploymentTargetResource> List(int skip = 0,
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