using System;
using System.Collections.Generic;

namespace Octopus.Client.Repositories
{
    public interface IGet<TResource>
    {
        TResource Get(string idOrHref);
        List<TResource> Get(params string[] ids);
        TResource Refresh(TResource resource);
    }
}