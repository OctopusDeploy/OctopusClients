using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IFindBySlug<TResource> : IPaginate<TResource> where TResource: IHaveSlugResource
    {
        TResource FindBySlug(string slug, string path = null, object pathParameters = null);
        List<TResource> FindBySlugs(IEnumerable<string> slugs, string path = null, object pathParameters = null);
    }
}