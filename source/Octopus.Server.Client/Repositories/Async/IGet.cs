using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories.Async
{
    public interface IGet<TResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TResource> Get(string idOrHref);
        Task<TResource> Get(string idOrHref, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<TResource>> Get(params string[] ids);
        Task<List<TResource>> Get(CancellationToken cancellationToken, params string[] ids);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TResource> Refresh(TResource resource);
        Task<TResource> Refresh(TResource resource, CancellationToken cancellationToken);
    }
}