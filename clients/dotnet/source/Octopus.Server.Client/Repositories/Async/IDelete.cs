using System;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories.Async
{
    public interface IDelete<in TResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task Delete(TResource resource);
        Task Delete(TResource resource, CancellationToken cancellationToken);
    }
}