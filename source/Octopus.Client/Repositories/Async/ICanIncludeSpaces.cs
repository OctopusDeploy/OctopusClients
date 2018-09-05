using System;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories.Async
{
    public interface ICanIncludeSpaces<out T>
    {
        T Including(SpaceContext spaceContext);
    }
}