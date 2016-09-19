using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IPaginate<TResource>
    {
        Task Paginate(Func<ResourceCollection<TResource>, bool> getNextPage, string path = null, object pathParameters = null);
        Task<TResource> FindOne(Func<TResource, bool> search, string path = null, object pathParameters = null);
        Task<List<TResource>> FindMany(Func<TResource, bool> search, string path = null, object pathParameters = null);
        Task<List<TResource>> FindAll(string path = null, object pathParameters = null);
    }
}