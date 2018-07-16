using System;
using System.Collections.Generic;
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
            ParameterHelper.ValidateSpaceParameters(SpaceQueryParameters, newParameters);
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
    }
}
