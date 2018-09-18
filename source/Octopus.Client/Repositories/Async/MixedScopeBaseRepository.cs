using System;
using System.Collections.Generic;
using Octopus.Client.Exceptions;
using Octopus.Client.Extensibility;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories.Async
{
    class MixedScopeBaseRepository<TMixedScopeResource>: BasicRepository<TMixedScopeResource> where TMixedScopeResource : class, IResource
    {
        readonly SpaceQueryContext spaceQueryContext;

        protected MixedScopeBaseRepository(IOctopusAsyncClient client, string collectionLinkName, SpaceQueryContext spaceQueryContext) : base(client, collectionLinkName)
        {
            this.spaceQueryContext = spaceQueryContext;
        }

        protected SpaceQueryContext CreateSpaceQueryContext(bool includeSystem, string[] spaceIds)
        {
            var newContext = new SpaceQueryContext(includeSystem, spaceIds);
            ValidateSpaceParameters(newContext);
            return newContext;
        }

        protected override Dictionary<string, object> AdditionalQueryParameters
        {
            get
            {
                if (spaceQueryContext == null)
                    return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    ["includeSystem"] = spaceQueryContext.IncludeSystem,
                    ["spaces"] = spaceQueryContext.SpaceIds
                };
            }
        }

        void ValidateSpaceParameters(SpaceQueryContext newSpaceQueryContext)
        {
            if (spaceQueryContext == null)
            {
                return;
            }

            if (newSpaceQueryContext.IncludeSystem && !spaceQueryContext.IncludeSystem)
            {
                throw new InvalidIncludeSystemConfigurationException();
            }

            var previouslyDefinedSpaceIdsSet = new HashSet<string>(spaceQueryContext.SpaceIds);
            if (!previouslyDefinedSpaceIdsSet.IsSupersetOf(newSpaceQueryContext.SpaceIds))
            {
                throw new InvalidSpacesLimitationException();
            }
        }
    }
}
