using System;
using System.Collections.Generic;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Repositories
{
    abstract class MixedScopeBaseRepository<TMixedScopeResource> : BasicRepository<TMixedScopeResource> where TMixedScopeResource : class, IResource
    {
        private readonly SpaceContext extendedSpaceContext;

        protected MixedScopeBaseRepository(IOctopusRepository repository, string collectionLinkName) : base(repository, _ => collectionLinkName)
        {
        }

        protected MixedScopeBaseRepository(IOctopusRepository repository, string collectionLinkName, SpaceContext includingSpaceContext, SpaceContext extendedSpaceContext) 
            : base(repository, _ => collectionLinkName)
        {
            this.extendedSpaceContext = extendedSpaceContext == null ? includingSpaceContext : extendedSpaceContext.Union(includingSpaceContext);
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
