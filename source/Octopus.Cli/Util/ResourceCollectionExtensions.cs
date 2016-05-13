using System;
using System.Collections.Generic;
using System.Linq;
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

        public static IEnumerable<TResource> GetAllPages<TResource>(this ResourceCollection<TResource> source, IOctopusRepository repository)
        {
            foreach (var item in source.Items)
                yield return item;

            while (source.HasNextPage())
            {
                source = repository.Client.List<TResource>(source.NextPageLink());
                foreach (var item in source.Items)
                    yield return item;
            }
        }

        public static void Paginate<TResource>(this ResourceCollection<TResource> source, IOctopusRepository repository, Func<ResourceCollection<TResource>, bool> getNextPage)
        {
            while (getNextPage(source) && source.Items.Count > 0 && source.HasNextPage())
                source = repository.Client.List<TResource>(source.NextPageLink());
        }

        public static TResource FindOne<TResource>(this ResourceCollection<TResource> source, IOctopusRepository repository, Func<TResource, bool> search)
        {
            var resource = default(TResource);
            source.Paginate(repository, page =>
            {
                resource = page.Items.FirstOrDefault(search);
                return resource == null;
            });
            return resource;
        }

        public static List<TResource> FindMany<TResource>(this ResourceCollection<TResource> source, IOctopusRepository repository, Func<TResource, bool> search)
        {
            var resources = new List<TResource>();
            source.Paginate(repository, page =>
            {
                resources.AddRange(page.Items.Where(search));
                return true;
            });
            return resources;
        }
    }
}