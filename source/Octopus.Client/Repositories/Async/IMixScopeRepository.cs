using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories.Async
{
    public interface IMixScopeRepository<TResource>
    {
        Task<List<TResource>> Search(bool includeGlobal, string[] spaceIds, object parameters = null);
    }
}