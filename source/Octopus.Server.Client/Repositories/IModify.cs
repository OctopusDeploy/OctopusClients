using System;

namespace Octopus.Client.Repositories
{
    public interface IModify<TResource>
    {
        TResource Modify(TResource resource);
    }
}