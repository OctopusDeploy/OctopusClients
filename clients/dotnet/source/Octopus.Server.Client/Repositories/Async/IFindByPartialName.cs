using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories.Async;

public interface IFindByPartialName<TResource> : IPaginate<TResource>
{
    Task<List<TResource>> FindByPartialName(string partialName, CancellationToken cancellationToken);
    Task<List<TResource>> FindByPartialName(string partialName, string path, object pathParameters, CancellationToken cancellationToken);
}