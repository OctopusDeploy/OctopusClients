using System;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories.Async
{
    public interface ICreate<TResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TResource> Create(TResource resource, object pathParameters = null);

        Task<TResource> Create(TResource resource, CancellationToken cancellationToken, object pathParameters = null);
    }
}