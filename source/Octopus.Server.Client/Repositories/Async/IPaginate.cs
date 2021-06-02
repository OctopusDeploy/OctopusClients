using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IPaginate<TResource>
    {
        Task Paginate(Func<ResourceCollection<TResource>, bool> getNextPage, string path = null, object pathParameters = null);
        Task<TResource> FindOne(Func<TResource, bool> search, string path = null, object pathParameters = null);
        Task<List<TResource>> FindMany(Func<TResource, bool> search, string path = null, object pathParameters = null);
        Task<List<TResource>> FindAll(string path = null, object pathParameters = null);
    }
}