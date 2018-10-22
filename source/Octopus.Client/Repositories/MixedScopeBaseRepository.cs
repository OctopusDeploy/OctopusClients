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

        protected override Dictionary<string, object> AdditionalQueryParameters
        {
            get
            {
                var combinedSpaceContext = extendedSpaceContext == null ? Repository.SpaceContext : Repository.SpaceContext.Union(extendedSpaceContext);
                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    [MixedScopeConstants.QueryStringParameterIncludeSystem] = combinedSpaceContext?.IncludeSystem ?? Repository.SpaceContext.IncludeSystem,
                    [MixedScopeConstants.QueryStringParameterSpaces] = combinedSpaceContext?.SpaceIds ?? Repository.SpaceContext.SpaceIds
                };

            }
        }

        protected SpaceContext GetCurrentSpaceContext()
        {
            return extendedSpaceContext ?? Repository.SpaceContext;
        }
    }
}
