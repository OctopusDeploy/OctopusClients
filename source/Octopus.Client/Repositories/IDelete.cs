using System;
using System.Threading.Tasks;

namespace Octopus.Client.Repositories
{
    public interface IDelete<in TResource>
    {
        Task Delete(TResource resource);
    }
}