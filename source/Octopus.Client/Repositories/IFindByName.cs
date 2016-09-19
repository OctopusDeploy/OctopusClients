using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Octopus.Client.Repositories
{
    public interface IFindByName<TResource> : IPaginate<TResource>
    {
        Task<TResource> FindByName(string name, string path = null, object pathParameters = null);
        Task<List<TResource>> FindByNames(IEnumerable<string> names, string path = null, object pathParameters = null);
    }
}