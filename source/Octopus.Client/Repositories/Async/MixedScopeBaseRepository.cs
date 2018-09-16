using System;
using System.Collections.Generic;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Repositories.Async
{
    class MixedScopeBaseRepository<TMixedScopeResource>: BasicRepository<TMixedScopeResource> where TMixedScopeResource : class, IResource
    {
        public MixedScopeBaseRepository(IOctopusAsyncClient client, string collectionLinkName) : base(client, collectionLinkName)
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
