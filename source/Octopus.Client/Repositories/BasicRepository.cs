#if SYNC_CLIENT
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
        public IOctopusRepository Repository { get; }
        readonly IOctopusClient client;
        protected readonly string CollectionLinkName;
        protected virtual Dictionary<string, object> AdditionalQueryParameters { get; }

        protected BasicRepository(IOctopusRepository repository, string collectionLinkName)
        {
            Repository = repository;
            client = repository.Client;
            CollectionLinkName = collectionLinkName;
            AdditionalQueryParameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public IOctopusClient Client => client;

        public TResource Create(TResource resource, object pathParameters = null)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            ValidateSpaceId(resource);
            EnrichSpaceIdIfRequire(resource);
            return client.Create(Repository.Link(CollectionLinkName), resource, pathParameters);
        }

        public TResource Modify(TResource resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            ValidateSpaceId(resource);
            return client.Update(resource.Links["Self"], resource);
        }

        public void Delete(TResource resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            ValidateSpaceId(resource);
            client.Delete(resource.Links["Self"]);
        }

        public void Paginate(Func<ResourceCollection<TResource>, bool> getNextPage, string path = null, object pathParameters = null)
        {
            var parameters = ParameterHelper.CombineParameters(AdditionalQueryParameters, pathParameters);
            client.Paginate(path ?? Repository.Link(CollectionLinkName), parameters, getNextPage);
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
            var parameters = ParameterHelper.CombineParameters(AdditionalQueryParameters, new { id = IdValueConstant.IdAll });
            return client.Get<List<TResource>>(Repository.Link(CollectionLinkName), parameters);
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
                return client.Get<TResource>(idOrHref, AdditionalQueryParameters);
            }
            var parameters = ParameterHelper.CombineParameters(AdditionalQueryParameters, new { id = idOrHref });
            return client.Get<TResource>(Repository.Link(CollectionLinkName), parameters);
        }

        public virtual List<TResource> Get(params string[] ids)
        {
            if (ids == null) return new List<TResource>();
            var actualIds = ids.Where(id => !string.IsNullOrWhiteSpace(id)).ToArray();
            if (actualIds.Length == 0) return new List<TResource>();

            var resources = new List<TResource>();
            var link = Repository.Link(CollectionLinkName);
            if(!Regex.IsMatch(link, @"\{\?.*\Wids\W"))
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

        void EnrichSpaceIdIfRequire(TResource resource)
        {
            if (resource is IHaveSpaceResource spaceResource && TypeUtil.IsAssignableToGenericType(this.GetType(), typeof(ICanExtendSpaceContext<>)))
            {
                if (IsInSingleSpaceContext())
                {
                    spaceResource.SpaceId = Repository.SpaceContext.SpaceIds.Single();
                }
            }
        }

        bool IsInSingleSpaceContext()
        {
            var spaceIds = GetSpaceIds();
            var isASpecificSpaceId = spaceIds.Length == 1 && !spaceIds.Contains(MixedScopeConstants.AllSpacesQueryStringParameterValue);
            return isASpecificSpaceId && !IsIncludingSystem();
        }

        string[] GetSpaceIds()
        {
            var isMixedScope = TypeUtil.IsAssignableToGenericType(this.GetType(), typeof(ICanExtendSpaceContext<>));
            return isMixedScope ? AdditionalQueryParameters[MixedScopeConstants.QueryStringParameterSpaces] as string[] : Repository.SpaceContext.SpaceIds.ToArray();
        }

        bool IsIncludingSystem()
        {
            var isMixedScope = TypeUtil.IsAssignableToGenericType(this.GetType(), typeof(ICanExtendSpaceContext<>));
            return isMixedScope
                ? bool.Parse(AdditionalQueryParameters[MixedScopeConstants.QueryStringParameterIncludeSystem].ToString())
                : Repository.SpaceContext.IncludeSystem;
        }

        void ValidateSpaceId(TResource resource)
        {
            if (resource is IHaveSpaceResource spaceResource)
            {
                var spaceIds = GetSpaceIds();
                var isWildcard = spaceIds != null && spaceIds.Contains(MixedScopeConstants.AllSpacesQueryStringParameterValue);
                if (isWildcard)
                    return;
                var contextDoesNotContainsSpaceIdFromResource = !string.IsNullOrEmpty(spaceResource.SpaceId) &&
                                                                spaceIds != null && !spaceIds.Contains(spaceResource.SpaceId);
                if (contextDoesNotContainsSpaceIdFromResource)
                {
                    throw new MismatchSpaceContextException("The space Id in the resource is not allowed in the current space context");
                }
            }
        }
    }

    // ReSharper restore MemberCanBePrivate.Local
    // ReSharper restore UnusedMember.Local
    // ReSharper restore MemberCanBeProtected.Local
}
#endif
