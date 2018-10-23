using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Repositories.Async
{
    class MixedScopeBaseRepository<TMixedScopeResource>: BasicRepository<TMixedScopeResource> where TMixedScopeResource : class, IResource
    {
        private readonly SpaceContext extendedSpaceContext;

        public MixedScopeBaseRepository(IOctopusAsyncRepository repository, string collectionLinkName) : base(repository, collectionLinkName)
        {
        }

        protected MixedScopeBaseRepository(IOctopusAsyncRepository repository, string collectionLinkName, SpaceContext includingSpaceContext, SpaceContext extendedSpaceContext) : base(repository,
            collectionLinkName)
        {
            this.extendedSpaceContext = extendedSpaceContext == null ? includingSpaceContext : extendedSpaceContext.Union(includingSpaceContext);
        }

        protected override Dictionary<string, object> AdditionalQueryParameters
        {
            get
            {
                ValidateExtension();
                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    [MixedScopeConstants.QueryStringParameterIncludeSystem] = CombineSpaceContext().IncludeSystem,
                    [MixedScopeConstants.QueryStringParameterSpaces] = CombineSpaceContext().SpaceIds
                };
            }
        }

        protected SpaceContext GetCurrentSpaceContext()
        {
            return extendedSpaceContext ?? Repository?.SpaceContext;
        }

        SpaceContext CombineSpaceContext()
        {
            return extendedSpaceContext == null ? Repository.SpaceContext : Repository.SpaceContext.Union(extendedSpaceContext);
        }

        void ValidateExtension()
        {
            if (Repository.SpaceContext.SpaceIds.Any() && CombineSpaceContext().SpaceIds.Count > 1)
                throw new SpaceContextExtensionException($"The Repository is scoped to {Repository.SpaceContext.SpaceIds.Single()}, you cannot include more spaces beyond the Repository scope");
        }
    }
}
