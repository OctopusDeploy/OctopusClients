using System;
using System.Collections.Generic;
using Octopus.Client.Extensibility;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories.Async
{
    class MixedScopeBaseRepository<TMixedScopeResource>: BasicRepository<TMixedScopeResource> where TMixedScopeResource : class, IResource
    {
        public MixedScopeBaseRepository(IOctopusAsyncClient client, string collectionLinkName) : base(client, collectionLinkName)
        {
            SetupSpaceParameters();
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


        void SetupSpaceParameters()
        {
            switch (Client.SpaceContext.SpaceSelection)
            {
                case SpaceSelection.SpecificSpaceAndSystem:
                    SpaceQueryParameters = new SpaceQueryParameters(true, new[] {Client.SpaceContext.SpaceId});
                    break;
                case SpaceSelection.DefaultSpaceAndSystem:
                    SpaceQueryParameters = new SpaceQueryParameters(true, new[] { "default" });
                    break;
                case SpaceSelection.SpecificSpace:
                    SpaceQueryParameters = new SpaceQueryParameters(false, new[] { Client.SpaceContext.SpaceId });
                    break;
                case SpaceSelection.SystemOnly:
                    SpaceQueryParameters = new SpaceQueryParameters(true, null);
                    break;
            }
        }
    }
}
