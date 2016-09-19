using System;
using System.Threading.Tasks;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ILifecyclesRepository : IGet<LifecycleResource>, ICreate<LifecycleResource>, IModify<LifecycleResource>, IDelete<LifecycleResource>, IFindByName<LifecycleResource>
    {
        Task<LifecycleEditor> CreateOrModify(string name);
        Task<LifecycleEditor> CreateOrModify(string name, string description);
    }
}