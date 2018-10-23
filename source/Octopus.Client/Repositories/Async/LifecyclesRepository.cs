using System;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ILifecyclesRepository : IGet<LifecycleResource>, ICreate<LifecycleResource>, IModify<LifecycleResource>, IDelete<LifecycleResource>, IFindByName<LifecycleResource>
    {
        Task<LifecycleEditor> CreateOrModify(string name);
        Task<LifecycleEditor> CreateOrModify(string name, string description);
    }

    class LifecyclesRepository : BasicRepository<LifecycleResource>, ILifecyclesRepository
    {
        public LifecyclesRepository(IOctopusAsyncRepository repository)
            : base(repository, "Lifecycles")
        {
        }

        public Task<LifecycleEditor> CreateOrModify(string name)
        {
            return new LifecycleEditor(this).CreateOrModify(name);
        }

        public Task<LifecycleEditor> CreateOrModify(string name, string description)
        {
            return new LifecycleEditor(this).CreateOrModify(name, description);
        }
    }
}
