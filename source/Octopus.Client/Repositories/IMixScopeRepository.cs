using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories
{
    public interface IMixScopeRepository<TResource>
    {
        List<TResource> Search(bool includeGlobal, params string[] spaceIds);
    }
}