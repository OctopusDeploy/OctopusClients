using System;

namespace Octopus.Client.Repositories
{
    public interface ICreate<TResource>
    {
        TResource Create(TResource resource, object pathParameters = null);
    }
}