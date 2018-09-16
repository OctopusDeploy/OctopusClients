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
        }

        protected SpaceContext ExtendedSpaceContext { get; set; }

        protected override Dictionary<string, object> AdditionalQueryParameters => new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            ["includeSystem"] = ExtendedSpaceContext?.IncludeSystem ?? Client.SpaceContext.IncludeSystem,
            ["spaces"] = ExtendedSpaceContext?.SpaceIds ?? Client.SpaceContext.SpaceIds
        };
    }
}
