using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Extensibility;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    abstract class MixedScopeBaseRepository<TMixedScopeResource> : BasicRepository<TMixedScopeResource> where TMixedScopeResource : class, IResource
    {
        private SpaceContext extendedSpaceContext;

        protected MixedScopeBaseRepository(IOctopusClient client, string collectionLinkName) : base(client, collectionLinkName)
        {
        }

        protected MixedScopeBaseRepository(IOctopusClient client, string collectionLinkName, SpaceContext spaceContext) : base(client, collectionLinkName)
        {
            extendedSpaceContext = spaceContext;
        }

        protected override Dictionary<string, object> AdditionalQueryParameters => new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            ["includeSystem"] = extendedSpaceContext?.IncludeSystem ?? Client.SpaceContext.IncludeSystem,
            ["spaces"] = extendedSpaceContext?.SpaceIds ?? Client.SpaceContext.SpaceIds
        };

        protected SpaceContext ExtendSpaceContext(SpaceContext includingSpaceContext)
        {
            if (extendedSpaceContext == null)
                extendedSpaceContext = new SpaceContext(Client.SpaceContext.SpaceIds, Client.SpaceContext.IncludeSystem);
            extendedSpaceContext = extendedSpaceContext.Union(includingSpaceContext);
            return extendedSpaceContext;
        }

        protected SpaceContext GetCurrentSpaceContext()
        {
            return extendedSpaceContext ?? Client.SpaceContext;
        }
    }
}
