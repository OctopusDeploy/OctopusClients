using System.Threading;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories.Async
{
    public interface IModify<TResource>
    {
        Task<TResource> Modify(TResource resource);

        Task<TResource> Modify(TResource resource, CancellationToken cancellationToken);
    }
}