using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Util;
using Octopus.Client.Validation;

namespace Octopus.Client.Repositories.Async
{
    // ReSharper disable MemberCanBePrivate.Local
    // ReSharper disable UnusedMember.Local
    // ReSharper disable MemberCanBeProtected.Local
    abstract class BasicRepository<TResource> where TResource : class, IResource
    {
        private readonly Func<IOctopusAsyncRepository, Task<string>> getCollectionLinkName;
        protected string CollectionLinkName;
        private SemanticVersion minimumRequiredVersion;
        private bool hasMinimumRequiredVersion;

        protected BasicRepository(IOctopusAsyncRepository repository, string collectionLinkName, Func<IOctopusAsyncRepository, Task<string>> getCollectionLinkName = null)
        {
            Client = repository.Client;
            Repository = repository;
            CollectionLinkName = collectionLinkName;
            this.getCollectionLinkName = getCollectionLinkName;
        }

        protected virtual Dictionary<string, object> GetAdditionalQueryParameters()
        {
            return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public IOctopusAsyncClient Client { get; }
        public IOctopusAsyncRepository Repository { get; }

        protected virtual void CheckSpaceResource(IHaveSpaceResource spaceResource)
        {
            Repository.Scope.Apply(
                whenSpaceScoped: space =>
                {
                    if (spaceResource.SpaceId != null && spaceResource.SpaceId != space.Id)
                        throw new ResourceSpaceDoesNotMatchRepositorySpaceException(spaceResource, space);
                },
                whenSystemScoped: () => { },
                whenUnspecifiedScope: () =>
                {
                    var spaceRoot = Repository.LoadSpaceRootDocument();
                    var isDefaultSpaceFound = spaceRoot != null;

                    if (!isDefaultSpaceFound)
                    {
                        throw new DefaultSpaceNotFoundException(spaceResource);
                    }
                });
        }

        protected void MinimumCompatibleVersion(string version)
        {
            minimumRequiredVersion = SemanticVersion.Parse(version);
            hasMinimumRequiredVersion = true;
        }

        private void AssertSpaceIdMatchesResource(TResource resource)
        {
            if (resource is IHaveSpaceResource spaceResource)
                CheckSpaceResource(spaceResource);
        }

        protected async Task<bool> ThrowIfServerVersionIsNotCompatible()
        {
            if (!hasMinimumRequiredVersion) return false;

            await EnsureServerIsMinimumVersion(
                minimumRequiredVersion,
                currentServerVersion => $"The version of the Octopus Server ('{currentServerVersion}') you are connecting to is not compatible with this version of Octopus.Client for this API call. Please upgrade your Octopus Server to a version greater than '{minimumRequiredVersion}'");

            return false;
        }

        protected async Task EnsureServerIsMinimumVersion(SemanticVersion requiredVersion, Func<string, string> messageGenerator)
        {
            var currentServerVersion = (await Repository.LoadRootDocument()).Version;

            if (ServerVersionCheck.IsOlderThanClient(currentServerVersion, requiredVersion))
            {
                throw new NotSupportedException(messageGenerator(currentServerVersion));
            }
        }

        public virtual async Task<TResource> Create(TResource resource, object pathParameters = null)
        {
            await ThrowIfServerVersionIsNotCompatible();

            var link = await ResolveLink().ConfigureAwait(false);
            AssertSpaceIdMatchesResource(resource);
            EnrichSpaceId(resource);
            return await Client.Create(link, resource, pathParameters).ConfigureAwait(false);
        }

        public virtual Task<TResource> Modify(TResource resource)
        {
            ThrowIfServerVersionIsNotCompatible().ConfigureAwait(false);

            AssertSpaceIdMatchesResource(resource);
            return Client.Update(resource.Links["Self"], resource);
        }

        public Task Delete(TResource resource)
        {
            ThrowIfServerVersionIsNotCompatible().ConfigureAwait(false);

            AssertSpaceIdMatchesResource(resource);
            return Client.Delete(resource.Links["Self"]);
        }

        public async Task Paginate(Func<ResourceCollection<TResource>, bool> getNextPage, string path = null, object pathParameters = null)
        {
            await ThrowIfServerVersionIsNotCompatible();

            var link = await ResolveLink().ConfigureAwait(false);
            var parameters = ParameterHelper.CombineParameters(GetAdditionalQueryParameters(), pathParameters);
            await Client.Paginate(path ?? link, parameters, getNextPage).ConfigureAwait(false);
        }

        public async Task<TResource> FindOne(Func<TResource, bool> search, string path = null, object pathParameters = null)
        {
            await ThrowIfServerVersionIsNotCompatible();

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
            await ThrowIfServerVersionIsNotCompatible();

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
            ThrowIfServerVersionIsNotCompatible().ConfigureAwait(false);

            return FindMany(r => true, path, pathParameters);
        }

        public async Task<List<TResource>> GetAll()
        {
            await ThrowIfServerVersionIsNotCompatible();

            var link = await ResolveLink().ConfigureAwait(false);
            var parameters = ParameterHelper.CombineParameters(GetAdditionalQueryParameters(), new { id = IdValueConstant.IdAll });
            return await Client.Get<List<TResource>>(link, parameters).ConfigureAwait(false);
        }

        public Task<TResource> FindByName(string name, string path = null, object pathParameters = null)
        {
            ThrowIfServerVersionIsNotCompatible().ConfigureAwait(false);

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
            ThrowIfServerVersionIsNotCompatible().ConfigureAwait(false);

            var nameSet = new HashSet<string>((names ?? new string[0]).Select(n => (n ?? string.Empty).Trim()), StringComparer.OrdinalIgnoreCase);
            return FindMany(r =>
            {
                var named = r as INamedResource;
                if (named != null) return nameSet.Contains((named.Name ?? string.Empty).Trim());
                return false;
            }, path, pathParameters);
        }

        public async Task<TResource> Get(string idOrHref)
        {
            await ThrowIfServerVersionIsNotCompatible();

            if (string.IsNullOrWhiteSpace(idOrHref))
                return null;

            var link = await ResolveLink().ConfigureAwait(false);
            var additionalQueryParameters = GetAdditionalQueryParameters();
            var parameters = ParameterHelper.CombineParameters(additionalQueryParameters, new { id = idOrHref });
            var  getTask = idOrHref.StartsWith("/", StringComparison.OrdinalIgnoreCase)
                ? Client.Get<TResource>(idOrHref, additionalQueryParameters).ConfigureAwait(false)
                : Client.Get<TResource>(link, parameters).ConfigureAwait(false);
            return await getTask;
        }

        public virtual async Task<List<TResource>> Get(params string[] ids)
        {
            await ThrowIfServerVersionIsNotCompatible();

            if (ids == null) return new List<TResource>();
            var actualIds = ids.Where(id => !string.IsNullOrWhiteSpace(id)).ToArray();
            if (actualIds.Length == 0) return new List<TResource>();

            var resources = new List<TResource>();

            var link = await ResolveLink().ConfigureAwait(false);
            if (!Regex.IsMatch(link, @"\{\?.*\Wids\W"))
                link += "{?ids}";

            var parameters = ParameterHelper.CombineParameters(GetAdditionalQueryParameters(), new { ids = actualIds });
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
            ThrowIfServerVersionIsNotCompatible().ConfigureAwait(false);

            if (resource == null) throw new ArgumentNullException("resource");
            return Get(resource.Id);
        }

        protected virtual void EnrichSpaceId(TResource resource)
        {
            ThrowIfServerVersionIsNotCompatible().ConfigureAwait(false);

            if (resource is IHaveSpaceResource spaceResource)
            {
                spaceResource.SpaceId = Repository.Scope.Apply(space => space.Id,
                    () => null,
                    () => spaceResource.SpaceId);
            }
        }

        protected async Task<string> ResolveLink()
        {
            await ThrowIfServerVersionIsNotCompatible();

            if (CollectionLinkName == null && getCollectionLinkName != null)
                CollectionLinkName = await getCollectionLinkName(Repository).ConfigureAwait(false);
            return await Repository.Link(CollectionLinkName).ConfigureAwait(false);
        }
    }

    // ReSharper restore MemberCanBePrivate.Local
    // ReSharper restore UnusedMember.Local
    // ReSharper restore MemberCanBeProtected.Local
}
