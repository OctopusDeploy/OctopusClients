using System;

namespace Octopus.Client.Repositories
{
    public interface ICanExtendSpaceContext<out T>
    {
        T UsingContext(SpaceContext userDefinedSpaceContext);
    }
}
