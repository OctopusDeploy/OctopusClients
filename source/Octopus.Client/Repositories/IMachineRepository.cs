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

        MachineEditor CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles,
            TenantResource[] tenants,
            TagResource[] tenantTags);

        MachineEditor CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles);
    }
}