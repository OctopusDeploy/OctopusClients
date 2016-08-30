using System;

namespace Octopus.Client.Repositories
{
    public interface IDelete<in TResource>
    {
        void Delete(TResource resource);
    }
}