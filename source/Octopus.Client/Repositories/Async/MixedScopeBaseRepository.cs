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

        protected override Dictionary<string, object> GetAdditionalQueryParameters()
        {
            var spaceContext = GetCurrentSpaceContext();
            return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                [MixedScopeConstants.QueryStringParameterIncludeSystem] = spaceContext.IncludeSystem,
                [MixedScopeConstants.QueryStringParameterSpaces] = spaceContext.ApplySpaceSelection<object>(spaces => spaces, () => "all")
            };
        }

        void ValidateThatICanUseACustomSpaceContext()
        {
            var scopeIsSpecified = Repository.Scope.Apply(_ => true, () => true, () => false);
            if (scopeIsSpecified)
            {
                throw new SpaceContextSwitchException();
            }
        }

        protected SpaceContext GetCurrentSpaceContext()
        {
            return extendedSpaceContext ?? Repository.Scope.Apply(SpaceContext.SpecificSpace, 
                       SpaceContext.SystemOnly, 
                       SpaceContext.AllSpacesAndSystem);
        }

        protected override void EnrichSpaceId(TMixedScopeResource resource)
        {
            base.EnrichSpaceId(resource);

            if (resource is IHaveSpaceResource spaceResource 
                && extendedSpaceContext != null)
            {
                spaceResource.SpaceId = extendedSpaceContext.ApplySpaceSelection(spaceIds =>
                {
                    if (spaceIds.Count == 1 && !extendedSpaceContext.IncludeSystem)
                    {
                        return spaceIds.Single();
                    }

                    if (spaceIds.Count == 0 && extendedSpaceContext.IncludeSystem)
                    {
                        return null;
                    }

                    return spaceResource.SpaceId;
                }, () => spaceResource.SpaceId);
            }
        }

        protected void EnsureSingleSpaceContext()
        {
            Repository.Scope.Apply(_ => {},
                () => throw new SpaceScopedOperationInSystemContextException(),
                () => 
                {
                    if (extendedSpaceContext == null)
                    {
                        return; // Assumes the default space
                    }

                    extendedSpaceContext.ApplySpaceSelection(spaces =>
                    {
                        var numberOfSpaces = spaces.Count;
                        if (numberOfSpaces == 0)
                        {
                            // We must be in a system context
                            throw new SpaceScopedOperationInSystemContextException();
                        }

                        if (numberOfSpaces > 1)
                        {
                            throw new SingleSpaceOperationInMultiSpaceContextException();
                        }
                    }, () => throw new SingleSpaceOperationInMultiSpaceContextException());
                });
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
