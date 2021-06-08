using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IGet<TResource>
    {
        TResource Get(string idOrHref);
        List<TResource> Get(params string[] ids);
        TResource Refresh(TResource resource);
    }

    public interface IGetProjectScoped<TResource>
    {
        TResource Get(ProjectResource projectResource, string idOrHref);
        List<TResource> Get(ProjectResource projectResource, params string[] ids);
    }
}