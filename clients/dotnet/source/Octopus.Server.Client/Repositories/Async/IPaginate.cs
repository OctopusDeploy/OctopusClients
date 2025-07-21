using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IPaginate<TResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task Paginate(Func<ResourceCollection<TResource>, bool> getNextPage, string path = null, object pathParameters = null);
        Task Paginate(Func<ResourceCollection<TResource>, bool> getNextPage, CancellationToken cancellationToken);
        Task Paginate(Func<ResourceCollection<TResource>, bool> getNextPage, string path, object pathParameters, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TResource> FindOne(Func<TResource, bool> search, string path = null, object pathParameters = null);
        Task<TResource> FindOne(Func<TResource, bool> search, CancellationToken cancellationToken);
        Task<TResource> FindOne(Func<TResource, bool> search, string path, object pathParameters, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<TResource>> FindMany(Func<TResource, bool> search, string path = null, object pathParameters = null);
        Task<List<TResource>> FindMany(Func<TResource, bool> search, CancellationToken cancellationToken);
        Task<List<TResource>> FindMany(Func<TResource, bool> search, string path, object pathParameters, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<TResource>> FindAll(string path = null, object pathParameters = null);
        Task<List<TResource>> FindAll(CancellationToken cancellationToken);
        Task<List<TResource>> FindAll(string path, object pathParameters, CancellationToken cancellationToken);
    }
}