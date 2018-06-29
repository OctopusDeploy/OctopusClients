using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Octopus.Client.Extensibility;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    partial class MixScopeResourceRepository<TMixScopedResource> : BasicRepository<TMixScopedResource> where TMixScopedResource : class, IResource
    {
        public MixScopeResourceRepository(IOctopusClient client, string collectionLinkName)
            : base(client, collectionLinkName)
        {

        }

        public List<TMixScopedResource> Search(bool includeGlobal, string[] spaceIds, object parameters = null)
        {
            var spaces = spaceIds.Where(id => !string.IsNullOrWhiteSpace(id)).ToArray();
            var resources = new List<TMixScopedResource>();
            var link = Client.RootDocument.Link(CollectionLinkName);

            var combinedParameters = ParameterHelper.CombineParameters(parameters, includeGlobal, spaces);
            Client.Paginate<TMixScopedResource>(
                    link,
                    combinedParameters,
                    page =>
                    {
                        resources.AddRange(page.Items);
                        return true;
                    });

            return resources;
        }

    }
}