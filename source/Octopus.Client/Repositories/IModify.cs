using System;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories
{
    public interface IModify<TResource>
    {
        Task<TResource> Modify(TResource resource);
    }
}