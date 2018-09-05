using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Extensibility;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories.Async
{
    class MixedScopeBaseRepository<TMixedScopeResource>: BasicRepository<TMixedScopeResource> where TMixedScopeResource : class, IResource
    {
        public MixedScopeBaseRepository(IOctopusAsyncClient client, string collectionLinkName) : base(client, collectionLinkName)
        {
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
        protected SpaceContext CreateSpaceContext(SpaceContext spaceContext)
        {
            return new SpaceContext(Client.SpaceContext.SpaceIds.Concat(spaceContext.SpaceIds).ToArray(), spaceContext.IncludeSystem);

        }
    }
}
