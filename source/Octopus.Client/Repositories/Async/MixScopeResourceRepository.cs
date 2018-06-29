using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Repositories.Async
{
    class MixScopeResourceRepository<TMixScopedResource> : BasicRepository<TMixScopedResource>, IMixScopeRepository<TMixScopedResource> where TMixScopedResource : class, IResource
    {
        public MixScopeResourceRepository(IOctopusAsyncClient client, string collectionLinkName)
            : base(client, collectionLinkName)
        {
            
        }

        public async Task<List<TMixScopedResource>> Search(bool includeGlobal, params string[] spaceIds)
        {
            var spaces = spaceIds.Where(id => !string.IsNullOrWhiteSpace(id)).ToArray();
            var resources = new List<TMixScopedResource>();
            var link = Client.RootDocument.Link(CollectionLinkName);
            if (!Regex.IsMatch(link, @"\{\?.*\Wspaces\W"))
                link += "{?spaces}";

            await Client.Paginate<TMixScopedResource>(
                    link,
                    new { spaces, includeGlobal },
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