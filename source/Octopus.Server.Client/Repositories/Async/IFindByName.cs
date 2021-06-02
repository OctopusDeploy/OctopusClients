using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories.Async
{
    public interface IFindByName<TResource> : IPaginate<TResource>
    {
        Task<TResource> FindByName(string name, string path = null, object pathParameters = null);
        Task<List<TResource>> FindByNames(IEnumerable<string> names, string path = null, object pathParameters = null);
    }
}