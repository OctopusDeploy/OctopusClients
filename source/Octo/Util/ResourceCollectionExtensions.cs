using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Util
{
    public static class ResourceCollectionExtensions
    {
        private const string PageNext = "Page.Next";

        public static bool HasNextPage<TResource>(this ResourceCollection<TResource> source)
        {
            return source.HasLink(PageNext);
        }

        public static string NextPageLink<TResource>(this ResourceCollection<TResource> source)
        {
            return source.Link(PageNext);
        }

        public static async Task<IEnumerable<TResource>> GetAllPages<TResource>(this ResourceCollection<TResource> source, IOctopusAsyncRepository repository)
        {
            var items = source.Items.ToList();

            while (source.HasNextPage())
            {
                source = await repository.Client.List<TResource>(source.NextPageLink()).ConfigureAwait(false);
                items.AddRange(source.Items);
            }
            return items;
        }

        public static async Task Paginate<TResource>(this ResourceCollection<TResource> source, IOctopusAsyncRepository repository, Func<ResourceCollection<TResource>, bool> getNextPage)
        {
            while (getNextPage(source) && source.Items.Count > 0 && source.HasNextPage())
                source = await repository.Client.List<TResource>(source.NextPageLink()).ConfigureAwait(false);
        }

        public static async Task<TResource> FindOne<TResource>(this ResourceCollection<TResource> source, IOctopusAsyncRepository repository, Func<TResource, bool> search)
        {
            var resource = default(TResource);
            await source.Paginate(repository, page =>
            {
                resource = page.Items.FirstOrDefault(search);
                return resource == null;
            })
            .ConfigureAwait(false);
            return resource;
        }

        public static async Task<List<TResource>> FindMany<TResource>(this ResourceCollection<TResource> source, IOctopusAsyncRepository repository, Func<TResource, bool> search)
        {
            var resources = new List<TResource>();
            await  source.Paginate(repository, page =>
            {
                resources.AddRange(page.Items.Where(search));
                return true;
            }).ConfigureAwait(false);
            return resources;
        }
    }
}