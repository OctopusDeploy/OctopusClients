using System;
using System.Collections.Generic;
using Octopus.Client.Exceptions;
using Octopus.Client.Extensibility;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    abstract class MixedScopeBaseRepository<TMixedScopeResource> : BasicRepository<TMixedScopeResource> where TMixedScopeResource : class, IResource
    {
        private readonly SpaceQueryContext spaceQueryContext;

        protected MixedScopeBaseRepository(IOctopusClient client, string collectionLinkName, SpaceQueryContext spaceQueryContext) : base(client, collectionLinkName)
        {
            this.spaceQueryContext = spaceQueryContext;
        }

        protected SpaceQueryContext CreateParameters(bool includeGlobal, string[] spaceIds)
        {
            var newParameters = new SpaceQueryContext(includeGlobal, spaceIds);
            ValidateSpaceParameters(newParameters);
            return newParameters;
        }

        protected override Dictionary<string, object> AdditionalQueryParameters
        {
            get
            {
                if (spaceQueryContext == null)return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                
                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    ["includeGlobal"] = spaceQueryContext.IncludeGlobal,
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

            if (newSpaceQueryContext.IncludeGlobal && !spaceQueryContext.IncludeGlobal)
            {
                throw new InvalidIncludeGlobalConfigurationException();
            }

            var previouslyDefinedSpaceIdsSet = new HashSet<string>(spaceQueryContext.SpaceIds);
            if (!previouslyDefinedSpaceIdsSet.IsSupersetOf(newSpaceQueryContext.SpaceIds))
            {
                throw new InvalidSpacesLimitationException();
            }
        }
    }
}
