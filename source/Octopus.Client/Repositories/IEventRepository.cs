using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IEventRepository : IGet<EventResource>
    {
        Task<ResourceCollection<EventResource>> List(int skip = 0, string filterByUserId = null, string regardingDocumentId = null, bool includeInternalEvents = false);
    }
}