using System;
using System.Collections.Generic;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Repositories.Async
{
    class MixedScopeBaseRepository<TMixedScopeResource>: BasicRepository<TMixedScopeResource> where TMixedScopeResource : class, IResource
    {
        private SpaceContext extendedSpaceContext;

        public MixedScopeBaseRepository(IOctopusAsyncClient client, string collectionLinkName) : base(client, collectionLinkName)
        {
        }

        protected MixedScopeBaseRepository(IOctopusAsyncClient client, string collectionLinkName, SpaceContext spaceContext) : base(client,
            collectionLinkName)
        {
            extendedSpaceContext = spaceContext;
        }

        protected override Dictionary<string, object> AdditionalQueryParameters => new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            [MixedScopeConstants.QueryStringParameterIncludeSystem] = extendedSpaceContext?.IncludeSystem ?? Client.SpaceContext.IncludeSystem,
            [MixedScopeConstants.QueryStringParameterSpaces] = extendedSpaceContext?.SpaceIds ?? Client.SpaceContext.SpaceIds
        };

        protected SpaceContext ExtendSpaceContext(SpaceContext includingSpaceContext)
        {
            if (extendedSpaceContext == null)
                extendedSpaceContext = new SpaceContext(Client.SpaceContext.SpaceIds, Client.SpaceContext.IncludeSystem);
            return extendedSpaceContext.Union(includingSpaceContext);
        }

        protected SpaceContext GetCurrentSpaceContext()
        {
            return extendedSpaceContext ?? Client.SpaceContext;
        }
    }
}
