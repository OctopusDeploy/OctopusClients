using System.Collections.Generic;
using Octopus.Client.Extensibility;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    abstract class MixedScopeBaseRepository<TMixedScopeResource> : BasicRepository<TMixedScopeResource> where TMixedScopeResource : class, IResource
    {
        protected MixedScopeBaseRepository(IOctopusClient client, string collectionLinkName) : base(client, collectionLinkName)
        {
        }

        SpaceQueryParameters SpaceQueryParameters { get; set; }
        protected void SetupParameters(bool includeGlobal, string[] spaceIds)
        {
            ParameterHelper.ValidateSpaceParameters(SpaceQueryParameters, includeGlobal, spaceIds);
            SpaceQueryParameters = new SpaceQueryParameters() { IncludeGlobal = includeGlobal, SpaceIds = spaceIds };
        }

        protected override Dictionary<string, object> AdditionalQueryParameters
        {
            get
            {
                if (SpaceQueryParameters == null)
                    return null;
                return new Dictionary<string, object>()
                {
                    ["includeGlobal"] = SpaceQueryParameters.IncludeGlobal,
                    ["spaces"] = SpaceQueryParameters.SpaceIds
                };
            }
        }
    }
}
