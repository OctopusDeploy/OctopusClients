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
    
    class LifecyclesRepository : BasicRepository<LifecycleResource>, ILifecyclesRepository
    {
        public LifecyclesRepository(IOctopusRepository repository)
            : base(repository, "Lifecycles")
        {
        }

        public LifecycleEditor CreateOrModify(string name)
        {
            return new LifecycleEditor(this).CreateOrModify(name);
        }

        public LifecycleEditor CreateOrModify(string name, string description)
        {
            return new LifecycleEditor(this).CreateOrModify(name, description);
        }
    }
}