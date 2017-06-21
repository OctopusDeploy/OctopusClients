#if SYNC_CLIENT
using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Model;
using System.Text.RegularExpressions;

namespace Octopus.Client.Repositories
{
    // ReSharper disable MemberCanBePrivate.Local
    // ReSharper disable UnusedMember.Local
    // ReSharper disable MemberCanBeProtected.Local
    abstract class BasicRepository<TResource> where TResource : class, IResource
    {
        readonly IOctopusClient client;
        protected readonly string CollectionLinkName;

        protected BasicRepository(IOctopusClient client, string collectionLinkName)
        {
            this.client = client;
            this.CollectionLinkName = collectionLinkName;
        }

        public IOctopusClient Client
        {
            get { return client; }
        }

        public TResource Create(TResource resource)
        {
            return client.Create(client.RootDocument.Link(CollectionLinkName), resource);
        }

        public TResource Modify(TResource resource)
        {
            return client.Update(resource.Links["Self"], resource);
        }

        public void Delete(TResource resource)
        {
            client.Delete(resource.Links["Self"]);
        }

        public void Paginate(Func<ResourceCollection<TResource>, bool> getNextPage, string path = null, object pathParameters = null)
        {
            client.Paginate(path ?? client.RootDocument.Link(CollectionLinkName), pathParameters ?? new { }, getNextPage);
        }

        public TResource FindOne(Func<TResource, bool> search, string path = null, object pathParameters = null)
        {
            TResource resource = null;
            Paginate(page =>
            {
                resource = page.Items.FirstOrDefault(search);
                return resource == null;
            }, path, pathParameters);
            return resource;
        }

        public List<TResource> FindMany(Func<TResource, bool> search, string path = null, object pathParameters = null)
        {
            var resources = new List<TResource>();
            Paginate(page =>
            {
                resources.AddRange(page.Items.Where(search));
                return true;
            }, path, pathParameters);
            return resources;
        }

        public List<TResource> FindAll(string path = null, object pathParameters = null)
        {
            return FindMany(r => true, path, pathParameters);
        }

        public List<TResource> GetAll()
        {
            return client.Get<List<TResource>>(client.RootDocument.Link(CollectionLinkName), new { id = "all" });
        }

        public TResource FindByName(string name, string path = null, object pathParameters = null)
        {
            name = (name ?? string.Empty).Trim();

            // Some endpoints allow a Name query param which greatly increases efficiency
            if (pathParameters == null)
                pathParameters = new {name = name};

            return FindOne(r =>
            {
                var named = r as INamedResource;
                if (named != null) return string.Equals((named.Name ?? string.Empty).Trim(), name, StringComparison.OrdinalIgnoreCase);
                return false;
            }, path, pathParameters);
        }

        public List<TResource> FindByNames(IEnumerable<string> names, string path = null, object pathParameters = null)
        {
            var nameSet = new HashSet<string>((names ?? new string[0]).Select(n => (n ?? string.Empty).Trim()), StringComparer.OrdinalIgnoreCase);
            return FindMany(r =>
            {
                var named = r as INamedResource;
                if (named != null) return nameSet.Contains((named.Name ?? string.Empty).Trim());
                return false;
            }, path, pathParameters);
        }

        public TResource Get(string idOrHref)
        {
            if (string.IsNullOrWhiteSpace(idOrHref))
                return null;

            if (idOrHref.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                return client.Get<TResource>(idOrHref);
            }

            return client.Get<TResource>(client.RootDocument.Link(CollectionLinkName), new { id = idOrHref });
        }

        public List<TResource> Get(params string[] ids)
        {
            if (ids == null) return new List<TResource>();
            var actualIds = ids.Where(id => !string.IsNullOrWhiteSpace(id)).ToArray();
            if (actualIds.Length == 0) return new List<TResource>();

            var resources = new List<TResource>();
            var link = client.RootDocument.Link(CollectionLinkName);
            if(!Regex.IsMatch(link, @"\{\?.*\Wids\W"))
                link += "{?ids}";
            client.Paginate<TResource>(
                link,
                new { ids = actualIds },
                page =>
                {
                    resources.AddRange(page.Items);
                    return true;
                });

            return resources;
        }

        public TResource Refresh(TResource resource)
        {
            if (resource == null) throw new ArgumentNullException("resource");
            return Get(resource.Id);
        }
    }

    // ReSharper restore MemberCanBePrivate.Local
    // ReSharper restore UnusedMember.Local
    // ReSharper restore MemberCanBeProtected.Local
}
#endif
