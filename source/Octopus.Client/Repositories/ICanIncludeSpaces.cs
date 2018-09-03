using System;

namespace Octopus.Client.Repositories
{
    public interface ICanIncludeSpaces<out T>
    {
        T Including(bool includeGlobal, params String[] spaceIds);
        T IncludingAllSpaces();
    }
}
