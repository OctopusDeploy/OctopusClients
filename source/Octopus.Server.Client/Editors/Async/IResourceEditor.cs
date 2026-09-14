using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Editors.Async
{
    public interface IResourceEditor<out TResource, TResourceBuilder> : IResourceBuilder
        where TResource : Resource
        where TResourceBuilder : IResourceBuilder
    {
        TResource Instance { get; }
        TResourceBuilder Customize(Action<TResource> customize);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TResourceBuilder> Save();
        Task<TResourceBuilder> Save(CancellationToken cancellationToken);
    }

    public interface IResourceBuilder
    {

    }
}
