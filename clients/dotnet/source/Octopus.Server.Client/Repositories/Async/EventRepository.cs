using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories.Async
{
    public interface IEventRepository : IGet<EventResource>, ICanExtendSpaceContext<IEventRepository>
    {
        [Obsolete("This method was deprecated in Octopus 3.4.  Please use the other List method by providing named arguments.")]
        Task<ResourceCollection<EventResource>> List(int skip = 0, 
            string filterByUserId = null,
            string regardingDocumentId = null,
            bool includeInternalEvents = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.15)</param>
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
        Task<ResourceCollection<EventResource>> List(int skip = 0,
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

        Task<IReadOnlyList<DocumentTypeResource>> GetDocumentTypes();
        Task<IReadOnlyList<EventAgentResource>> GetAgents();
        Task<IReadOnlyList<EventCategoryResource>> GetCategories();
        Task<IReadOnlyList<EventGroupResource>> GetGroups();
    }

    class EventRepository : MixedScopeBaseRepository<EventResource>, IEventRepository
    {
        public EventRepository(IOctopusAsyncRepository repository)
            : base(repository, "Events")
        {
        }

        EventRepository(IOctopusAsyncRepository repository, SpaceContext userDefinedSpaceContext)
            : base(repository, "Events", userDefinedSpaceContext)
        {
        }

        [Obsolete("This method was deprecated in Octopus 3.4.  Please use the other List method by providing named arguments.")]
        public async Task<ResourceCollection<EventResource>> List(int skip = 0, 
                string filterByUserId = null,
                string regardingDocumentId = null,
                bool includeInternalEvents = false)
        {
            return await Client.List<EventResource>(await Repository.Link("Events").ConfigureAwait(false), ParameterHelper.CombineParameters(GetAdditionalQueryParameters(), new
            {
                skip,
                user = filterByUserId,
                regarding = regardingDocumentId,
                @internal = includeInternalEvents.ToString()
            })).ConfigureAwait(false);
        }

        public async Task<ResourceCollection<EventResource>> List(int skip = 0, 
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
            var parameters = ParameterHelper.CombineParameters(GetAdditionalQueryParameters(), new
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

            return await Client.List<EventResource>(await Repository.Link("Events").ConfigureAwait(false), parameters).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<DocumentTypeResource>> GetDocumentTypes()
        {
            var link = await Repository.Link("EventDocumentTypes").ConfigureAwait(false);
            return await Client.Get<List<DocumentTypeResource>>(link).ConfigureAwait(false);
        }
        
        public async Task<IReadOnlyList<EventAgentResource>> GetAgents()
        {
            var link = await Repository.Link("EventAgents").ConfigureAwait(false);
            return await Client.Get<List<EventAgentResource>>(link).ConfigureAwait(false);
        }
        
        public async Task<IReadOnlyList<EventCategoryResource>> GetCategories()        
        {
            var link = await Repository.Link("EventCategories").ConfigureAwait(false);
            return await Client.Get<List<EventCategoryResource>>(link).ConfigureAwait(false);
        }
        
        public async Task<IReadOnlyList<EventGroupResource>> GetGroups()
        {
            var link = await Repository.Link("EventGroups").ConfigureAwait(false);
            return await Client.Get<List<EventGroupResource>>(link).ConfigureAwait(false);
        }

        public IEventRepository UsingContext(SpaceContext spaceContext)
        {
            return new EventRepository(Repository, spaceContext);
        }
    }
}