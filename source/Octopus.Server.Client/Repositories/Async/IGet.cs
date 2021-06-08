using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IGet<TResource>
    {
        Task<TResource> Get(string idOrHref);
        Task<List<TResource>> Get(params string[] ids);
        Task<TResource> Refresh(TResource resource);
    }

    public interface IGetProjectScoped<TResource>
    {
        Task<TResource> Get(ProjectResource projectResource, string idOrHref);
    }
}