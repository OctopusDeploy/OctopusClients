using System;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ILifecyclesRepository : IGet<LifecycleResource>, ICreate<LifecycleResource>, IModify<LifecycleResource>, IDelete<LifecycleResource>, IFindByName<LifecycleResource>
    {
        LifecycleEditor CreateOrModify(string name);
        LifecycleEditor CreateOrModify(string name, string description);
    }
}