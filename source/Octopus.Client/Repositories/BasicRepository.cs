using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Model;
using System.Text.RegularExpressions;
using Octopus.Client.Exceptions;
using Octopus.Client.Extensibility;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    // ReSharper disable MemberCanBePrivate.Local
    // ReSharper disable UnusedMember.Local
    // ReSharper disable MemberCanBeProtected.Local
    abstract class BasicRepository<TResource> where TResource : class, IResource
    {
        private readonly Func<IOctopusRepository, string> getCollectionLinkName;
        public IOctopusRepository Repository { get; }
        readonly IOctopusClient client;
        protected string CollectionLinkName;
        protected virtual Dictionary<string, object> AdditionalQueryParameters { get; }

        protected BasicRepository(IOctopusRepository repository, string collectionLinkName, Func<IOctopusRepository, string> getCollectionLinkName = null)
        {
            Repository = repository;
            client = repository.Client;
            CollectionLinkName = collectionLinkName;
            AdditionalQueryParameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this.getCollectionLinkName = getCollectionLinkName;
        }

        public IOctopusClient Client => client;

        private void AssertSpaceIdMatchesResource(TResource resource, bool isEmptySpaceAllowed = false)
        {
            if (resource is IHaveSpaceResource spaceResource)
            {
                Repository.Scope
                    .Apply(space =>
                        {
                            var errorMessageTemplate = $"The resource has a different space specified than the one specified by the repository scope. Either change the {nameof(IHaveSpaceResource.SpaceId)} on the resource to {space.Id}, or use a repository that is scoped to";

                            if (isEmptySpaceAllowed && string.IsNullOrWhiteSpace(spaceResource.SpaceId))
                                return (string) null;
                            
                            if (string.IsNullOrWhiteSpace(spaceResource.SpaceId) && !space.IsDefault)
                                throw new ArgumentException(
                                    $"{errorMessageTemplate} the default space.");

                            if (!string.IsNullOrWhiteSpace(spaceResource.SpaceId) && spaceResource.SpaceId != space.Id)
                                throw new ArgumentException(
                                    $"{errorMessageTemplate} {spaceResource.SpaceId}.");
                            
                            return (string) null;
                        },
                    () => null,
                    () => null);
            }
        }
        
        public virtual TResource Create(TResource resource, object pathParameters = null)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            AssertSpaceIdMatchesResource(resource, isEmptySpaceAllowed: true);
            
            var link = ResolveLink();
            EnrichSpaceId(resource);
            return client.Create(link, resource, pathParameters);
        }

        public virtual TResource Modify(TResource resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            AssertSpaceIdMatchesResource(resource);
            return client.Update(resource.Links["Self"], resource);
        }

        public void Delete(TResource resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            AssertSpaceIdMatchesResource(resource);
            client.Delete(resource.Links["Self"]);
        }

        public void Paginate(Func<ResourceCollection<TResource>, bool> getNextPage, string path = null, object pathParameters = null)
        {
            var link = ResolveLink();
            var parameters = ParameterHelper.CombineParameters(AdditionalQueryParameters, pathParameters);
            client.Paginate(path ?? link, parameters, getNextPage);
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
            var link = ResolveLink();
            var parameters = ParameterHelper.CombineParameters(AdditionalQueryParameters, new { id = IdValueConstant.IdAll });
            return client.Get<List<TResource>>(link, parameters);
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
            var link = ResolveLink();
            if (idOrHref.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                return client.Get<TResource>(idOrHref, AdditionalQueryParameters);
            }
            var parameters = ParameterHelper.CombineParameters(AdditionalQueryParameters, new { id = idOrHref });
            return client.Get<TResource>(link, parameters);
        }

        public virtual List<TResource> Get(params string[] ids)
        {
            if (ids == null) return new List<TResource>();
            var actualIds = ids.Where(id => !string.IsNullOrWhiteSpace(id)).ToArray();
            if (actualIds.Length == 0) return new List<TResource>();

            var resources = new List<TResource>();
            var link = ResolveLink();
            if (!Regex.IsMatch(link, @"\{\?.*\Wids\W"))
                link += "{?ids}";
            var parameters = ParameterHelper.CombineParameters(AdditionalQueryParameters, new { ids = actualIds });
            client.Paginate<TResource>(
                link,
                parameters,
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

        protected virtual void EnrichSpaceId(TResource resource)
        {
            if (resource is IHaveSpaceResource spaceResource)
            {
                spaceResource.SpaceId = Repository.Scope.Apply(space => space.Id,
                    () => null,
                    () => spaceResource.SpaceId);
            }
        }

        protected string ResolveLink()
        {
            if (CollectionLinkName == null && getCollectionLinkName != null)
                CollectionLinkName = getCollectionLinkName(Repository);
            return Repository.Link(CollectionLinkName);
        }
    }

    // ReSharper restore MemberCanBePrivate.Local
    // ReSharper restore UnusedMember.Local
    // ReSharper restore MemberCanBeProtected.Local
}