using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Extensibility;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    abstract class MixedScopeBaseRepository<TMixedScopeResource> : BasicRepository<TMixedScopeResource> where TMixedScopeResource : class, IResource
    {
        protected MixedScopeBaseRepository(IOctopusClient client, string collectionLinkName) : base(client, collectionLinkName)
        {
            SpaceContextExtension = new SpaceContextExtension(client.SpaceContext.IncludeSystem, client.SpaceContext.SpaceIds.ToArray());
        }

        protected SpaceContextExtension SpaceContextExtension { get; set; }

        protected override Dictionary<string, object> AdditionalQueryParameters
        {
            get
            {
                if (SpaceContextExtension == null)
                    return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
                {
                    ["includeGlobal"] = SpaceContextExtension.IncludeSystem,
                    ["spaces"] = SpaceContextExtension.SpaceIds
                };
            }
        }
    }
}
