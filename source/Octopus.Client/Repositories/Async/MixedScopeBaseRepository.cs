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
            SpaceContext = new SpaceContext(client.SpaceContext.SpaceIds.ToArray(), client.SpaceContext.IncludeSystem);
        }

        protected SpaceContext SpaceContext { get; set; }

        protected override Dictionary<string, object> AdditionalQueryParameters
        {
            get
            {
                if (SpaceContext == null)
                    return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    // Server has not yet changed to includeSystem yet
                    ["includeGlobal"] = SpaceContext.IncludeSystem,
                    ["spaces"] = SpaceContext.SpaceIds
                };
            }
        }
    }
}
