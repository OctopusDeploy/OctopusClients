using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Util;
using Octopus.Client.Repositories;

namespace Octopus.Client.Repositories.Async
{
    // ReSharper disable MemberCanBePrivate.Local
    // ReSharper disable UnusedMember.Local
    // ReSharper disable MemberCanBeProtected.Local
    abstract class BasicRepository<TResource> where TResource : class, IResource
    {
        protected readonly string CollectionLinkName;
        protected virtual Dictionary<string, object> AdditionalQueryParameters { get; }

        protected BasicRepository(IOctopusAsyncRepository repository, string collectionLinkName)
        {
            this.Client = repository.Client;
            Repository = repository;
            this.CollectionLinkName = collectionLinkName;
            AdditionalQueryParameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public IOctopusAsyncClient Client { get; }
        public IOctopusAsyncRepository Repository { get; }

        public Task<TResource> Create(TResource resource, object pathParameters = null)
        {
            ValidateSpaceId(resource);
            EnrichSpaceIdIfRequire(resource);
            return Client.Create(Repository.Link(CollectionLinkName), resource, pathParameters);
        }

        public Task<TResource> Modify(TResource resource)
        {
            ValidateSpaceId(resource);
            return Client.Update(resource.Links["Self"], resource);
        }

        public Task Delete(TResource resource)
        {
            ValidateSpaceId(resource);
            return Client.Delete(resource.Links["Self"]);
        }

        public Task Paginate(Func<ResourceCollection<TResource>, bool> getNextPage, string path = null, object pathParameters = null)
        {
            var parameters = ParameterHelper.CombineParameters(AdditionalQueryParameters, pathParameters);
            return Client.Paginate(path ?? Repository.Link(CollectionLinkName), parameters, getNextPage);
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
            var parameters = ParameterHelper.CombineParameters(AdditionalQueryParameters, new { id = IdValueConstant.IdAll });
            return Client.Get<List<TResource>>(Repository.Link(CollectionLinkName), parameters);
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

            var parameters = ParameterHelper.CombineParameters(AdditionalQueryParameters, new { id = idOrHref });
            return idOrHref.StartsWith("/", StringComparison.OrdinalIgnoreCase)
                ? Client.Get<TResource>(idOrHref, AdditionalQueryParameters)
                : Client.Get<TResource>(Repository.Link(CollectionLinkName), parameters);
        }

        public virtual async Task<List<TResource>> Get(params string[] ids)
        {
            if (ids == null) return new List<TResource>();
            var actualIds = ids.Where(id => !string.IsNullOrWhiteSpace(id)).ToArray();
            if (actualIds.Length == 0) return new List<TResource>();

            var resources = new List<TResource>();
            var link = Repository.Link(CollectionLinkName);
            if (!Regex.IsMatch(link, @"\{\?.*\Wids\W"))
                link += "{?ids}";

            var parameters = ParameterHelper.CombineParameters(AdditionalQueryParameters, new { ids = actualIds });
            await Client.Paginate<TResource>(
                link,
                parameters,
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
