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
            var scopeIsUnspecified = Repository.Scope.Apply(_ => false, () => false, () => true);
            if (scopeIsUnspecified)
            {
                throw new SpaceContextSwitchException();
            }
        }

        protected async Task<SpaceContext> GetCurrentSpaceContext()
        {
            return extendedSpaceContext ?? await Repository.Scope.ToSpaceContext(Repository);
        }
    }

    public class SpaceContextSwitchException : Exception
    {
        public SpaceContextSwitchException() : base(
            "Cannot switch to a custom space context when the repository has already been explicitly scoped to a context. " +
            "Use client.Repository to obtain a repository that has NOT been explicitly scoped to a context")
        {
        }
    }
}
