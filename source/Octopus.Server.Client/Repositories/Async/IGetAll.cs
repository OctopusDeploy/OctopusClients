using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories.Async
{
    public interface IGetAll<TResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<TResource>> GetAll();
        Task<List<TResource>> GetAll(CancellationToken cancellationToken);
    }
}