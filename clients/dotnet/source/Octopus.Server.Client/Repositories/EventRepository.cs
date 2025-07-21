using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    public interface IEventRepository : IGet<EventResource>, ICanExtendSpaceContext<IEventRepository>
    {
        [Obsolete("This method was deprecated in Octopus 3.4.  Please use the other List method by providing named arguments.")]
        ResourceCollection<EventResource> List(int skip = 0, 
            string filterByUserId = null,
            string regardingDocumentId = null,
            bool includeInternalEvents = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.159)</param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="regarding"></param>
        /// <param name="regardingAny"></param>
        /// <param name="includeInternalEvents"></param>
        /// <param name="user"></param>
        /// <param name="users"></param>
        /// <param name="projects"></param>
        /// <param name="environments"></param>
        /// <param name="eventGroups"></param>
        /// <param name="eventCategories"></param>
        /// <param name="tenants"></param>
        /// <param name="tags"></param>
        /// <param name="fromAutoId"></param>
        /// <param name="toAutoId"></param>
        /// <param name="documentTypes"></param>
        /// <param name="eventAgents"></param>
        /// <param name="projectGroups"></param>
        /// <returns></returns>
        ResourceCollection<EventResource> List(int skip = 0, 
            int? take = null,
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
            string documentTypes = null,
            string eventAgents = null,
            string projectGroups = null);

        IReadOnlyList<DocumentTypeResource> GetDocumentTypes();
        IReadOnlyList<EventAgentResource> GetAgents();
        IReadOnlyList<EventCategoryResource> GetCategories();
        IReadOnlyList<EventGroupResource> GetGroups();
    }
    
    class EventRepository : MixedScopeBaseRepository<EventResource>, IEventRepository
    {
        public EventRepository(IOctopusRepository repository)
            : base(repository, "Events")
        {
        }

        EventRepository(IOctopusRepository repository, SpaceContext userDefinedSpaceContext)
            : base(repository, "Events", userDefinedSpaceContext)
        {
        }

        [Obsolete("This method was deprecated in Octopus 3.4.  Please use the other List method by providing named arguments.")]
        public ResourceCollection<EventResource> List(int skip = 0, 
            string filterByUserId = null,
            string regardingDocumentId = null,
            bool includeInternalEvents = false)
        {
            return Client.List<EventResource>(Repository.Link("Events"), new
            {
                skip,
                user = filterByUserId,
                regarding = regardingDocumentId,
                @internal = includeInternalEvents.ToString()
            });
        }

        public ResourceCollection<EventResource> List(int skip = 0, 
            int? take = null,
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
            string documentTypes = null,
            string eventAgents = null,
            string projectGroups = null)
        {
            var parameters = ParameterHelper.CombineParameters(AdditionalQueryParameters, new
            {
                skip,
                take,
                from,
                to,
                regarding,
                regardingAny,
                @internal = includeInternalEvents,
                user,
                users,
                projects,
                environments,
                eventGroups,
                eventCategories,
                tenants,
                tags,
                fromAutoId,
                toAutoId,
                documentTypes,
                eventAgents,
                projectGroups,
            });
            return Client.List<EventResource>(Repository.Link("Events"), parameters);
        }
        
        public IReadOnlyList<DocumentTypeResource> GetDocumentTypes()
        {
            var link = Repository.Link("EventDocumentTypes");
            return Client.Get<List<DocumentTypeResource>>(link);
        }
        
        public IReadOnlyList<EventAgentResource> GetAgents()
        {
            var link = Repository.Link("EventAgents");
            return Client.Get<List<EventAgentResource>>(link);
        }
        
        public IReadOnlyList<EventCategoryResource> GetCategories()        
        {
            var link = Repository.Link("EventCategories");
            return Client.Get<List<EventCategoryResource>>(link);
        }
        
        public IReadOnlyList<EventGroupResource> GetGroups()
        {
            var link = Repository.Link("EventGroups");
            return Client.Get<List<EventGroupResource>>(link);
        }

        public IEventRepository UsingContext(SpaceContext userDefinedSpaceContext)
        {
            return new EventRepository(Repository, userDefinedSpaceContext);
        }
    }
}