using System.Threading.Tasks;

namespace Octopus.Client.Repositories.Async
{
    public interface ICanExtendSpaceContext<out T>
    {
        T UsingContext(SpaceContext spaceContext);
    }
}