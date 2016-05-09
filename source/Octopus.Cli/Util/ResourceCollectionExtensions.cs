using System.Collections.Generic;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Util
{
    public static class ResourceCollectionExtensions
    {
        private const string PageNext = "Page.Next";

        public static bool HasNextPage<T>(this ResourceCollection<T> source)
        {
            return source.HasLink(PageNext);
        }

        public static string NextPageLink<T>(this ResourceCollection<T> source)
        {
            return source.Link(PageNext);
        }

        public static IEnumerable<T> GetAllPages<T>(this ResourceCollection<T> source, IOctopusRepository repository)
        {
            foreach (var item in source.Items)
                yield return item;

            while (source.HasNextPage())
            {
                source = repository.Client.List<T>(source.NextPageLink());
                foreach (var item in source.Items)
                    yield return item;
            }
        }
    }
}