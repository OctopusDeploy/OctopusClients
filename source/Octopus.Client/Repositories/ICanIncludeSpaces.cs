using System;

namespace Octopus.Client.Repositories
{
    public interface ICanIncludeSpaces<out T>
    {
        T Including(SpaceContext spaceContext);
    }
}
