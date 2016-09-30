using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IEventRepository : IGet<EventResource>
    {
        Task<ResourceCollection<EventResource>> List(int skip = 0, string filterByUserId = null, string regardingDocumentId = null, bool includeInternalEvents = false);
    }

    class EventRepository : BasicRepository<EventResource>, IEventRepository
    {
        public EventRepository(IOctopusAsyncClient client)
            : base(client, "Events")
        {
        }

        public Task<ResourceCollection<EventResource>> List(int skip = 0, string filterByUserId = null, string regardingDocumentId = null, bool includeInternalEvents = false)
        {
            return Client.List<EventResource>(Client.RootDocument.Link("Events"), new { skip, user = filterByUserId, regarding = regardingDocumentId, @internal = includeInternalEvents.ToString() });
        }
    }
}