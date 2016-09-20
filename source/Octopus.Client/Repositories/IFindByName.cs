using System;
using System.Collections.Generic;

namespace Octopus.Client.Repositories
{
    public interface IFindByName<TResource> : IPaginate<TResource>
    {
        TResource FindByName(string name, string path = null, object pathParameters = null);
        List<TResource> FindByNames(IEnumerable<string> names, string path = null, object pathParameters = null);
    }
}