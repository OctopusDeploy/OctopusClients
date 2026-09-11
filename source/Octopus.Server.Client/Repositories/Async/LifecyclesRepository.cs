using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ILifecyclesRepository : IGet<LifecycleResource>, ICreate<LifecycleResource>, IModify<LifecycleResource>, IDelete<LifecycleResource>, IFindByName<LifecycleResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<LifecycleEditor> CreateOrModify(string name);
        Task<LifecycleEditor> CreateOrModify(string name, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<LifecycleEditor> CreateOrModify(string name, string description);
        Task<LifecycleEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken);
    }

    class LifecyclesRepository : BasicRepository<LifecycleResource>, ILifecyclesRepository
    {
        public LifecyclesRepository(IOctopusAsyncRepository repository)
            : base(repository, "Lifecycles")
        {
        }

        public Task<LifecycleEditor> CreateOrModify(string name)
        {
            return CreateOrModify(name, CancellationToken.None);
        }

        public Task<LifecycleEditor> CreateOrModify(string name, CancellationToken cancellationToken)
        {
            return new LifecycleEditor(this).CreateOrModify(name);
        }

        public Task<LifecycleEditor> CreateOrModify(string name, string description)
        {
            return CreateOrModify(name, description, CancellationToken.None);
        }

        public Task<LifecycleEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken)
        {
            return new LifecycleEditor(this).CreateOrModify(name, description);
        }
    }
}
