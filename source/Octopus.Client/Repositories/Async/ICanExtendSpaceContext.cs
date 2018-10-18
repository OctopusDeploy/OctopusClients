using System.Threading.Tasks;

namespace Octopus.Client.Repositories.Async
{
    public interface ICanExtendSpaceContext<T>
    {
        Task<T> Including(SpaceContext spaceContext);
    }
}