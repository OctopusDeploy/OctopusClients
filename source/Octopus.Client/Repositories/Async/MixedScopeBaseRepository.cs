using System;
using System.Collections.Generic;
using Octopus.Client.Exceptions;
using Octopus.Client.Extensibility;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories.Async
{
    class MixedScopeBaseRepository<TMixedScopeResource>: BasicRepository<TMixedScopeResource> where TMixedScopeResource : class, IResource
    {
        public MixedScopeBaseRepository(IOctopusAsyncClient client, string collectionLinkName, SpaceQueryContext spaceQueryContext) : base(client, collectionLinkName)
        {
            SpaceQueryContext = spaceQueryContext;
        }

        protected SpaceQueryContext CreateParameters(bool includeGlobal, string[] spaceIds)
        {
            var newParameter = new SpaceQueryContext(includeGlobal, spaceIds);
            ValidateSpaceParameters(newParameter);
            return newParameter;
        }
        protected SpaceQueryContext SpaceQueryContext { get; set; }

        protected override Dictionary<string, object> AdditionalQueryParameters
        {
            get
            {
                if (SpaceQueryContext == null)
                    return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    ["includeGlobal"] = SpaceQueryContext.IncludeGlobal,
                    ["spaces"] = SpaceQueryContext.SpaceIds
                };
            }
        }

        void ValidateSpaceParameters(SpaceQueryContext newSpaceQueryContext)
        {
            if (SpaceQueryContext == null)
            {
                return;
            }

            if (newSpaceQueryContext.IncludeGlobal && !SpaceQueryContext.IncludeGlobal)
            {
                throw new InvalidIncludeGlobalConfigurationException();
            }

            var previouslyDefinedSpaceIdsSet = new HashSet<string>(SpaceQueryContext.SpaceIds);
            if (!previouslyDefinedSpaceIdsSet.IsSupersetOf(newSpaceQueryContext.SpaceIds))
            {
                throw new InvalidSpacesLimitationException();
            }
        }
    }
}
