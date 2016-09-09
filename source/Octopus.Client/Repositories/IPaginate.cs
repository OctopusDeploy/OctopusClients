using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IPaginate<TResource>
    {
        void Paginate(Func<ResourceCollection<TResource>, bool> getNextPage, string path = null, object pathParameters = null);
        TResource FindOne(Func<TResource, bool> search, string path = null, object pathParameters = null);
        List<TResource> FindMany(Func<TResource, bool> search, string path = null, object pathParameters = null);
        List<TResource> FindAll(string path = null, object pathParameters = null);
    }
}