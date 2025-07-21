using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories.Async
{
    public interface IFindByName<TResource> : IPaginate<TResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TResource> FindByName(string name, string path = null, object pathParameters = null);
        Task<TResource> FindByName(string name, CancellationToken cancellationToken);
        Task<TResource> FindByName(string name, string path, object pathParameters, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<TResource>> FindByNames(IEnumerable<string> names, string path = null, object pathParameters = null);
        Task<List<TResource>> FindByNames(IEnumerable<string> names, CancellationToken cancellationToken);
        Task<List<TResource>> FindByNames(IEnumerable<string> names, string path, object pathParameters, CancellationToken cancellationToken);
    }
}