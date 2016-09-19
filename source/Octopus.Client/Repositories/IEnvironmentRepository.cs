using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IEnvironmentRepository : IFindByName<EnvironmentResource>, IGet<EnvironmentResource>, ICreate<EnvironmentResource>, IModify<EnvironmentResource>, IDelete<EnvironmentResource>, IGetAll<EnvironmentResource>
    {
        List<MachineResource> GetMachines(EnvironmentResource environment);
        Task Sort(string[] environmentIdsInOrder);
        Task<EnvironmentEditor> CreateOrModify(string name);
        Task<EnvironmentEditor> CreateOrModify(string name, string description);
    }
}