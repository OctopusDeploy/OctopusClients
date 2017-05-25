using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IEventRepository : IGet<EventResource>
    {
        ResourceCollection<EventResource> List(int skip = 0,
            string filterByUserId = null,
            string regardingDocumentId = null,
            bool includeInternalEvents = false);

        ResourceCollection<EventResource> List(int skip = 0,
            string from = null,
            string to = null,
            string regarding = null,
            string regardingAny = null,
            bool includeInternalEvents = true,
            string user = null,
            string users = null,
            string projects = null,
            string environments = null,
            string eventGroups = null,
            string eventCategories = null,
            string tenants = null,
            string tags = null,
            long? fromAutoId = null,
            long? toAutoId = null,
            string documentTypes = null);
    }
    
    class EventRepository : BasicRepository<EventResource>, IEventRepository
    {
        public EventRepository(IOctopusClient client)
            : base(client, "Events")
        {
        }

        public ResourceCollection<EventResource> List(int skip = 0,
            string filterByUserId = null,
            string regardingDocumentId = null,
            bool includeInternalEvents = false)
        {
            return Client.List<EventResource>(Client.RootDocument.Link("Events"), new
            {
                skip,
                user = filterByUserId,
                regarding = regardingDocumentId,
                @internal = includeInternalEvents.ToString()
            });
        }

        public ResourceCollection<EventResource> List(int skip = 0,
            string from = null,
            string to = null,
            string regarding = null,
            string regardingAny = null,
            bool includeInternalEvents = true,
            string user = null,
            string users = null,
            string projects = null,
            string environments = null,
            string eventGroups = null,
            string eventCategories = null,
            string tenants = null,
            string tags = null,
            long? fromAutoId = null,
            long? toAutoId = null,
            string documentTypes = null)
        {
            return Client.List<EventResource>(Client.RootDocument.Link("Events"), new
            {
                skip,
                from = from,
                to = to,
                regarding = regarding,
                regardingAny = regardingAny,
                @internal = includeInternalEvents,
                user = user,
                users = users,
                projects = projects,
                environments = environments,
                eventGroups = eventGroups,
                eventCategories = eventCategories,
                tenants = tenants,
                tags = tags,
                fromAutoId = fromAutoId,
                toAutoId = toAutoId,
                documentTypes = documentTypes
            });
        }
    }
}