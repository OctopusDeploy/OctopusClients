using System;

namespace Octopus.Client.Repositories
{
    public interface ICanExpandSpaceContext<out T>
    {
        T Including(bool includeGlobal, params String[] spaceIds);
    }
}
