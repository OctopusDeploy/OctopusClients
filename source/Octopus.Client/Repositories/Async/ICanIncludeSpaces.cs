using System;

namespace Octopus.Client.Repositories.Async
{
    public interface ICanIncludeSpaces<out T>
    {
        T Including(bool includeGlobal, params string[] spaceIds);
        T IncludingAllSpaces();
    }
}