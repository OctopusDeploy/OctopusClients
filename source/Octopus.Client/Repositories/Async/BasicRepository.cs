using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable UnusedMember.Local
        // ReSharper disable MemberCanBeProtected.Local
        abstract class BasicRepository<TResource> where TResource : class, IResource
        {
            protected readonly string CollectionLinkName;

            protected BasicRepository(IOctopusAsyncClient client, string collectionLinkName)
            {
                this.Client = client;
                this.CollectionLinkName = collectionLinkName;
            }

            public IOctopusAsyncClient Client { get; }

            public Task<TResource> Create(TResource resource)
            {
                return Client.Create(Client.RootDocument.Link(CollectionLinkName), resource);
            }

            public Task<TResource> Modify(TResource resource)
            {
                return Client.Update(resource.Links["Self"], resource);
            }

            public Task Delete(TResource resource)
            {
                return Client.Delete(resource.Links["Self"]);
            }

            public Task Paginate(Func<ResourceCollection<TResource>, bool> getNextPage, string path = null, object pathParameters = null)
            {
                return Client.Paginate(path ?? Client.RootDocument.Link(CollectionLinkName), pathParameters ?? new { }, getNextPage);
            }

            public async Task<TResource> FindOne(Func<TResource, bool> search, string path = null, object pathParameters = null)
            {
                TResource resource = null;
                await Paginate(page =>
                {
                    resource = page.Items.FirstOrDefault(search);
                    return resource == null;
                }, path, pathParameters)
                    .ConfigureAwait(false);
                return resource;
            }

            public async Task<List<TResource>> FindMany(Func<TResource, bool> search, string path = null, object pathParameters = null)
            {
                var resources = new List<TResource>();
                await Paginate(page =>
                {
                    resources.AddRange(page.Items.Where(search));
                    return true;
                }, path, pathParameters)
                    .ConfigureAwait(false);
                return resources;
            }

            public Task<List<TResource>> FindAll(string path = null, object pathParameters = null)
            {
                return FindMany(r => true, path, pathParameters);
            }

            public Task<List<TResource>> GetAll()
            {
                return Client.Get<List<TResource>>(Client.RootDocument.Link(CollectionLinkName), new { id = "all" });
            }

            public Task<TResource> FindByName(string name, string path = null, object pathParameters = null)
            {
                name = (name ?? string.Empty).Trim();

                // Some endpoints allow a Name query param which greatly increases efficiency
                if (pathParameters == null)
                    pathParameters = new { name = name };

                return FindOne(r =>
                {
                    var named = r as INamedResource;
                    if (named != null) return string.Equals((named.Name ?? string.Empty).Trim(), name, StringComparison.OrdinalIgnoreCase);
                    return false;
                }, path, pathParameters);
            }

            public Task<List<TResource>> FindByNames(IEnumerable<string> names, string path = null, object pathParameters = null)
            {
                var nameSet = new HashSet<string>((names ?? new string[0]).Select(n => (n ?? string.Empty).Trim()), StringComparer.OrdinalIgnoreCase);
                return FindMany(r =>
                {
                    var named = r as INamedResource;
                    if (named != null) return nameSet.Contains((named.Name ?? string.Empty).Trim());
                    return false;
                }, path, pathParameters);
            }

            public Task<TResource> Get(string idOrHref)
            {
                if (string.IsNullOrWhiteSpace(idOrHref))
                    return null;

                return idOrHref.StartsWith("/", StringComparison.OrdinalIgnoreCase)
                    ? Client.Get<TResource>(idOrHref)
                    : Client.Get<TResource>(Client.RootDocument.Link(CollectionLinkName), new { id = idOrHref });
            }

            public async Task<List<TResource>> Get(params string[] ids)
            {
                if (ids == null) return new List<TResource>();
                var actualIds = ids.Where(id => !string.IsNullOrWhiteSpace(id)).ToArray();
                if (actualIds.Length == 0) return new List<TResource>();

                var resources = new List<TResource>();
                var link = Client.RootDocument.Link(CollectionLinkName);
                if (!Regex.IsMatch(link, @"\{\?.*\Wids\W"))
                    link += "{?ids}";

                await Client.Paginate<TResource>(
                    link,
                    new { ids = actualIds },
                    page =>
                    {
                        resources.AddRange(page.Items);
                        return true;
                    })
                    .ConfigureAwait(false);

                return resources;
            }

            public Task<TResource> Refresh(TResource resource)
            {
                if (resource == null) throw new ArgumentNullException("resource");
                return Get(resource.Id);
            }
        }

        // ReSharper restore MemberCanBePrivate.Local
        // ReSharper restore UnusedMember.Local
        // ReSharper restore MemberCanBeProtected.Local
}
