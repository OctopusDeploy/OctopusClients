using System.Collections.Generic;

namespace Octopus.Client.Repositories;

public interface IFindByPartialName<TResource> : IPaginate<TResource>
{
     List<TResource> FindByPartialName(string partialName, string path = null, object pathParameters = null);
}