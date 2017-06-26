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
        MachineConnectionStatus GetConnectionStatus(MachineResource machine);
        List<MachineResource> FindByThumbprint(string thumbprint);
        IReadOnlyList<TaskResource> GetTasks(MachineResource machine);

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
    }
    
    class MachineRepository : BasicRepository<MachineResource>, IMachineRepository
    {
        public MachineRepository(IOctopusClient client) : base(client, "Machines")
        {
        }

        public MachineResource Discover(string host, int port = 10933, DiscoverableEndpointType? type = null)
        {
            return Client.Get<MachineResource>(Client.RootDocument.Link("DiscoverMachine"), new { host, port, type });
        }

        public MachineConnectionStatus GetConnectionStatus(MachineResource machine)
        {
            if (machine == null) throw new ArgumentNullException("machine");
            return Client.Get<MachineConnectionStatus>(machine.Link("Connection"));
        }

        public List<MachineResource> FindByThumbprint(string thumbprint)
        {
            if (thumbprint == null) throw new ArgumentNullException("thumbprint");
            return Client.Get<List<MachineResource>>(Client.RootDocument.Link("machines"), new { id = "all", thumbprint });
        }

        public IReadOnlyList<TaskResource> GetTasks(MachineResource machine)
        {
            if (machine == null) throw new ArgumentNullException(nameof(machine));
            return Client.ListAll<TaskResource>(machine.Link("TasksTemplate"), new {skip = 0});
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
    }
}