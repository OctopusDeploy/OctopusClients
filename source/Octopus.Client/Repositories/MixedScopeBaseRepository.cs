using System;
using System.Collections.Generic;
using Octopus.Client.Exceptions;
using Octopus.Client.Extensibility;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    abstract class MixedScopeBaseRepository<TMixedScopeResource> : BasicRepository<TMixedScopeResource> where TMixedScopeResource : class, IResource
    {
        private readonly SpaceQueryParameters spaceQueryParameters;

        protected MixedScopeBaseRepository(IOctopusClient client, string collectionLinkName, SpaceQueryParameters spaceQueryParameters) : base(client, collectionLinkName)
        {
            this.spaceQueryParameters = spaceQueryParameters;
        }

        protected SpaceQueryParameters CreateParameters(bool includeGlobal, string[] spaceIds)
        {
            var newParameters = new SpaceQueryParameters(includeGlobal, spaceIds);
            ValidateSpaceParameters(newParameters);
            return newParameters;
        }

        protected override Dictionary<string, object> AdditionalQueryParameters
        {
            get
            {
                if (spaceQueryParameters == null)return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                
                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    ["includeGlobal"] = spaceQueryParameters.IncludeGlobal,
                    ["spaces"] = spaceQueryParameters.SpaceIds
                };
            }
        }

        void ValidateSpaceParameters(SpaceQueryParameters newSpaceQueryParameters)
        {
            if (spaceQueryParameters == null)
            {
                return;
            }

            if (newSpaceQueryParameters.IncludeGlobal && !spaceQueryParameters.IncludeGlobal)
            {
                throw new InvalidIncludeGlobalConfigurationException();
            }

            var previouslyDefinedSpaceIdsSet = new HashSet<string>(spaceQueryParameters.SpaceIds);
            if (!previouslyDefinedSpaceIdsSet.IsSupersetOf(newSpaceQueryParameters.SpaceIds))
            {
                throw new InvalidSpacesLimitationException();
            }
        }
    }
}
