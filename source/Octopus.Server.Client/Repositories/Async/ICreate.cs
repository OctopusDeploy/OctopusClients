using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ICreate<TResource>
    {
        Task<TResource> Create(TResource resource, object pathParameters = null);
    }
}