using System;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories
{
    public interface ICreate<TResource>
    {
        Task<TResource> Create(TResource resource);
    }
}