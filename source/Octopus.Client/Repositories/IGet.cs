using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Octopus.Client.Repositories
{
    public interface IGet<TResource>
    {
        Task<TResource> Get(string idOrHref);
        Task<List<TResource>> Get(params string[] ids);
        Task<TResource> Refresh(TResource resource);
    }
}