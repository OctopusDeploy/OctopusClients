using System;

namespace Octopus.Client.Repositories.Async
{
    public interface ICanExpandSpaceContext<out T>
    {
        T Including(bool includeGlobal, params string[] spaceIds);
    }
}