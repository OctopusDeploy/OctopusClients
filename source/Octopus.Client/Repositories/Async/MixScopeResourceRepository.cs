using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Octopus.Client.Extensibility;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories.Async
{
    class MixScopeResourceRepository<TMixScopedResource> : BasicRepository<TMixScopedResource> where TMixScopedResource : class, IResource
    {
        public MixScopeResourceRepository(IOctopusAsyncClient client, string collectionLinkName)
            : base(client, collectionLinkName)
        {
            
        }

        public async Task<List<TMixScopedResource>> Search(bool includeGlobal, string[] spaceIds, object parameters = null)
        {
            var spaces = spaceIds.Where(id => !string.IsNullOrWhiteSpace(id)).ToArray();
            var resources = new List<TMixScopedResource>();
            var link = Client.RootDocument.Link(CollectionLinkName);
            if (!Regex.IsMatch(link, @"\{\?.*\Wspaces\W"))
                link += "{?spaces}";

            var combinedParameters = ParameterHelper.CombineParameters(parameters, includeGlobal, spaces);
            await Client.Paginate<TMixScopedResource>(
                    link,
                    combinedParameters,
                    page =>
                    {
                        resources.AddRange(page.Items);
                        return true;
                    })
                .ConfigureAwait(false);

            return resources;
        }
    }
}