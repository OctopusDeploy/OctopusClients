using System;
using System.Collections.Generic;
using Octopus.Client.Exceptions;
using Octopus.Client.Extensibility;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    abstract class MixedScopeBaseRepository<TMixedScopeResource> : BasicRepository<TMixedScopeResource> where TMixedScopeResource : class, IResource
    {
        protected MixedScopeBaseRepository(IOctopusClient client, string collectionLinkName, SpaceQueryParameters spaceQueryParameters) : base(client, collectionLinkName)
        {
            SpaceQueryParameters = spaceQueryParameters;
        }

        protected SpaceQueryParameters CreateParameters(bool includeGlobal, string[] spaceIds)
        {
            var newParameters = new SpaceQueryParameters(includeGlobal, spaceIds);
            ValidateSpaceParameters(newParameters);
            return newParameters;
        }
        protected SpaceQueryParameters SpaceQueryParameters { get; set; }

        protected override Dictionary<string, object> AdditionalQueryParameters
        {
            get
            {
                if (SpaceQueryParameters == null)
                    return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    ["includeGlobal"] = SpaceQueryParameters.IncludeGlobal,
                    ["spaces"] = SpaceQueryParameters.SpaceIds
                };
            }
        }

        void ValidateSpaceParameters(SpaceQueryParameters newSpaceQueryParameters)
        {
            if (SpaceQueryParameters == null)
            {
                return;
            }

            if (newSpaceQueryParameters.IncludeGlobal && !SpaceQueryParameters.IncludeGlobal)
            {
                throw new InvalidIncludeGlobalConfigurationException();
            }

            var previouslyDefinedSpaceIdsSet = new HashSet<string>(SpaceQueryParameters.SpaceIds);
            if (!previouslyDefinedSpaceIdsSet.IsSupersetOf(newSpaceQueryParameters.SpaceIds))
            {
                throw new InvalidSpacesLimitationException();
            }
        }
    }
}
