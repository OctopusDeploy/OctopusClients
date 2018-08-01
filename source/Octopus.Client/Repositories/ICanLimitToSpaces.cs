using System;

namespace Octopus.Client.Repositories
{
    public interface ICanLimitToSpaces<out T>
    {
        T LimitTo(bool includeGlobal, params String[] spaceIds);
    }
}
