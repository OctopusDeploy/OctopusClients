using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IFindBySlug<TResource> : IPaginate<TResource> where TResource: IHaveSlugResource
    {
        Task<TResource> FindBySlug(string slug, CancellationToken cancellationToken);
        Task<TResource> FindBySlug(string slug, string path, object pathParameters, CancellationToken cancellationToken);
        
        Task<List<TResource>> FindBySlugs(IEnumerable<string> slugs, CancellationToken cancellationToken);
        Task<List<TResource>> FindBySlugs(IEnumerable<string> slugs, string path, object pathParameters, CancellationToken cancellationToken);
    }
}