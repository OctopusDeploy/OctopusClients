using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Repositories.Async
{
    class MixedScopeBaseRepository<TMixedScopeResource> : BasicRepository<TMixedScopeResource> where TMixedScopeResource : class, IResource
    {
        private readonly SpaceContext extendedSpaceContext;

        public MixedScopeBaseRepository(IOctopusAsyncRepository repository, string collectionLinkName) : base(repository, collectionLinkName)
        {
        }

        protected MixedScopeBaseRepository(IOctopusAsyncRepository repository, string collectionLinkName, SpaceContext userDefinedSpaceContext) : base(repository,
            collectionLinkName)
        {
            ValidateThatICanUseACustomSpaceContext();
            this.extendedSpaceContext = userDefinedSpaceContext;
        }

        protected override async Task<Dictionary<string, object>> GetAdditionalQueryParameters()
        {
            var spaceContext = await GetCurrentSpaceContext();
            return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                [MixedScopeConstants.QueryStringParameterIncludeSystem] = spaceContext.IncludeSystem,
                [MixedScopeConstants.QueryStringParameterSpaces] = spaceContext.SpaceIds
            };
        }

        void ValidateThatICanUseACustomSpaceContext()
        {
            if (Repository.Scope.Type != RepositoryScope.RepositoryScopeType.Unspecified)
            {
                throw new Exception(
                    "Can't use custom context if you have set up an explicit context for your repository");
            }
        }

        protected async Task<SpaceContext> GetCurrentSpaceContext()
        {
            return extendedSpaceContext ?? await Repository.Scope.ToSpaceContext(Repository);
        }
    }
}
