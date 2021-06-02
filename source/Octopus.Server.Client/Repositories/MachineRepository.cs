using System;
using System.Collections.Generic;
using Octopus.Client.Editors;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Repositories
{
    public interface IMachineRepository : IFindByName<MachineResource>, IGet<MachineResource>, ICreate<MachineResource>, IModify<MachineResource>, IDelete<MachineResource>
    {
        MachineResource Discover(string host, int port = 10933, DiscoverableEndpointType? discoverableEndpointType = null);
        MachineResource Discover(DiscoverMachineOptions options);
        MachineConnectionStatus GetConnectionStatus(MachineResource machine);
        List<MachineResource> FindByThumbprint(string thumbprint);
        IReadOnlyList<TaskResource> GetTasks(MachineResource machine);
        IReadOnlyList<TaskResource> GetTasks(MachineResource machine, object pathParameters);


        MachineEditor CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles,
            TenantResource[] tenants,
            TagResource[] tenantTags,
            TenantedDeploymentMode? tenantedDeploymentParticipation);

        MachineEditor CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles);
        
        ResourceCollection<MachineResource> List(int skip = 0,
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
    
    class MachineRepository : BasicRepository<MachineResource>, IMachineRepository
    {
        public MachineRepository(IOctopusRepository repository) : base(repository, "Machines")
        {
        }

        public MachineResource Discover(string host, int port = 10933, DiscoverableEndpointType? type = null)
            => Discover(new DiscoverMachineOptions(host)
            {
                Port = port,
                Type = type
            });

        public MachineResource Discover(DiscoverMachineOptions options)
            => Client.Get<MachineResource>(Repository.Link("DiscoverMachine"), new
            {
                host = options.Host,
                port = options.Port,
                type = options.Type,
                proxyId = options.Proxy?.Id
            });

        public MachineConnectionStatus GetConnectionStatus(MachineResource machine)
        {
            if (machine == null) throw new ArgumentNullException("machine");
            return Client.Get<MachineConnectionStatus>(machine.Link("Connection"));
        }

        public List<MachineResource> FindByThumbprint(string thumbprint)
        {
            if (thumbprint == null) throw new ArgumentNullException("thumbprint");
            return Client.Get<List<MachineResource>>(Repository.Link("machines"), new { id = IdValueConstant.IdAll, thumbprint });
        }

        /// <summary>
        /// Gets all tasks involving the specified machine
        /// </summary>
        /// <param name="machine"></param>
        /// <returns></returns>
        public IReadOnlyList<TaskResource> GetTasks(MachineResource machine) => GetTasks(machine, new {skip = 0});

        /// <summary>
        /// Gets all tasks involving the specified machine
        /// 
        /// The `take` pathParmeter is only respected in Octopus 4.0.6 and later
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="pathParameters"></param>
        /// <returns></returns>
        public IReadOnlyList<TaskResource> GetTasks(MachineResource machine, object pathParameters)
        {
            if (machine == null)
                throw new ArgumentNullException(nameof(machine));

            return Client.ListAll<TaskResource>(machine.Link("TasksTemplate"), pathParameters);
        }

        public MachineEditor CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles,
            TenantResource[] tenants,
            TagResource[] tenantTags,
            TenantedDeploymentMode? tenantedDeploymentParticipation)
        {
            return new MachineEditor(this).CreateOrModify(name, endpoint, environments, roles, tenants, tenantTags, tenantedDeploymentParticipation);
        }

        public MachineEditor CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles)
        {
            return new MachineEditor(this).CreateOrModify(name, endpoint, environments, roles);
        }

        public ResourceCollection<MachineResource> List(int skip = 0,
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
            return Client.List<MachineResource>(Repository.Link("Machines"), new
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