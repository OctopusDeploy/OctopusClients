using System;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories.Async
{
    public interface IModify<TResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TResource> Modify(TResource resource);

        Task<TResource> Modify(TResource resource, CancellationToken cancellationToken);
    }
}