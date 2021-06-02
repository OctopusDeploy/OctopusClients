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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pathParameters">Server 3.14.159 and later support the `take` argument</param>
        /// <returns></returns>
        List<TResource> FindAll(string path = null, object pathParameters = null);
    }
}