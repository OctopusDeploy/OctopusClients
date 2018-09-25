using System;
using System.Collections.Generic;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Repositories
{
    abstract class MixedScopeBaseRepository<TMixedScopeResource> : BasicRepository<TMixedScopeResource> where TMixedScopeResource : class, IResource
    {
        private SpaceContext extendedSpaceContext;

        protected MixedScopeBaseRepository(IOctopusRepository repository, string collectionLinkName) : base(repository, collectionLinkName)
        {
        }

        protected MixedScopeBaseRepository(IOctopusRepository repository, string collectionLinkName, SpaceContext spaceContext) : base(repository, collectionLinkName)
        {
            extendedSpaceContext = spaceContext;
        }

        protected override Dictionary<string, object> AdditionalQueryParameters => new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            [MixedScopeConstants.QueryStringParameterIncludeSystem] = extendedSpaceContext?.IncludeSystem ?? Repository.SpaceContext.IncludeSystem,
            [MixedScopeConstants.QueryStringParameterSpaces] = extendedSpaceContext?.SpaceIds ?? Repository.SpaceContext.SpaceIds
        };

        protected SpaceContext ExtendSpaceContext(SpaceContext includingSpaceContext)
        {
            if (extendedSpaceContext == null)
                extendedSpaceContext = new SpaceContext(Repository.SpaceContext.SpaceIds, Repository.SpaceContext.IncludeSystem);
            return extendedSpaceContext.Union(includingSpaceContext);
        }

        protected SpaceContext GetCurrentSpaceContext()
        {
            return extendedSpaceContext ?? Repository.SpaceContext;
        }
    }
}
