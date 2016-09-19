using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Octopus.Client.Editors;
using Octopus.Client.Model;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Repositories
{
    public interface IMachineRepository : IFindByName<MachineResource>, IGet<MachineResource>, ICreate<MachineResource>, IModify<MachineResource>, IDelete<MachineResource>
    {
        Task<MachineResource> Discover(string host, int port = 10933, DiscoverableEndpointType? discoverableEndpointType = null);
        Task<MachineConnectionStatus> GetConnectionStatus(MachineResource machine);
        Task<List<MachineResource>> FindByThumbprint(string thumbprint);

        Task<MachineEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles,
            TenantResource[] tenants,
            TagResource[] tenantTags);

        Task<MachineEditor> CreateOrModify(
            string name,
            EndpointResource endpoint,
            EnvironmentResource[] environments,
            string[] roles);
    }
}