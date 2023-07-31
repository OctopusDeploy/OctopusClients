using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensions;
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

        protected virtual Task CheckSpaceResource(IHaveSpaceResource spaceResource)
        {
            return Repository.Scope.Apply(
                whenSpaceScoped: space =>
                {
                    if (spaceResource.SpaceId != null && spaceResource.SpaceId != space.Id)
                        throw new ResourceSpaceDoesNotMatchRepositorySpaceException(spaceResource, space);
                    return Task.FromResult(0);
                },
                whenSystemScoped: () => Task.FromResult(0),
                whenUnspecifiedScope: async () =>
                {
                    var spaceRoot = await Repository.LoadSpaceRootDocument().ConfigureAwait((false));
                    var isDefaultSpaceFound = spaceRoot != null;

                    if (!isDefaultSpaceFound && await ServerSupportsSpaces().ConfigureAwait(false))
                    {
                        throw new DefaultSpaceNotFoundException(spaceResource);
                    }
                });
        }

        private async Task<bool> ServerSupportsSpaces()
        {
            var rootDocument = await Repository.LoadRootDocument().ConfigureAwait(false);

            var spacesIsSupported = rootDocument.HasLink("Spaces");

            return spacesIsSupported;
        }

        protected void MinimumCompatibleVersion(string version)
        {
            minimumRequiredVersion = SemanticVersion.Parse(version);
            hasMinimumRequiredVersion = true;
        }

        private async Task AssertSpaceIdMatchesResource(TResource resource)
        {
            if (resource is IHaveSpaceResource spaceResource)
                await CheckSpaceResource(spaceResource).ConfigureAwait(false);
        }

        protected async Task<bool> ThrowIfServerVersionIsNotCompatible(CancellationToken cancellationToken)
        {
            if (!hasMinimumRequiredVersion) return false;

            await EnsureServerIsMinimumVersion(
                minimumRequiredVersion,
                currentServerVersion => $"The version of the Octopus Server ('{currentServerVersion}') you are connecting to is not compatible with this version of Octopus.Client for this API call. Please upgrade your Octopus Server to a version greater than '{minimumRequiredVersion}'",
                cancellationToken);

            return false;
        }

        protected async Task EnsureServerIsMinimumVersion(SemanticVersion requiredVersion, Func<string, string> messageGenerator, CancellationToken cancellationToken)
        {
            var currentServerVersion = (await Repository.LoadRootDocument(cancellationToken)).Version;

            if (ServerVersionCheck.IsOlderThanClient(currentServerVersion, requiredVersion))
            {
                throw new NotSupportedException(messageGenerator(currentServerVersion));
            }
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public virtual async Task<TResource> Create(TResource resource, object pathParameters = null)
        {
            return await Create(resource, pathParameters, CancellationToken.None);
        }
        
        public virtual async Task<TResource> Create(TResource resource, CancellationToken cancellationToken)
        {
            return await Create(resource, null, cancellationToken);
        }

        public virtual async Task<TResource> Create(TResource resource,  object pathParameters, CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken);

            var link = await ResolveLink(cancellationToken).ConfigureAwait(false);
            await AssertSpaceIdMatchesResource(resource).ConfigureAwait(false);
            EnrichSpaceId(resource);
            return await Client.Create(link, resource, pathParameters, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public virtual async Task<TResource> Modify(TResource resource)
        {
            return await Modify(resource, CancellationToken.None);
        }

        public virtual async Task<TResource> Modify(TResource resource, CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken).ConfigureAwait(false);
            
            await AssertSpaceIdMatchesResource(resource).ConfigureAwait(false);
            return await Client.Update(resource.Links["Self"], resource, null, cancellationToken).ConfigureAwait(false);
        }

        public async Task Delete(TResource resource)
        {
            await Delete(resource, CancellationToken.None);
        }
        
        public async Task Delete(TResource resource, CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken).ConfigureAwait(false);

            await AssertSpaceIdMatchesResource(resource).ConfigureAwait(false);
            
            await Client.Delete(resource.Links["Self"], cancellationToken).ConfigureAwait(false);
        }

        public async Task Paginate(Func<ResourceCollection<TResource>, bool> getNextPage, string path = null, object pathParameters = null)
        {
            await Paginate(getNextPage, path, pathParameters, CancellationToken.None);
        }

        public async Task Paginate(Func<ResourceCollection<TResource>, bool> getNextPage, CancellationToken cancellationToken)
        {
            await Paginate(getNextPage, path: null, pathParameters: null, cancellationToken);
        }

        public async Task Paginate(Func<ResourceCollection<TResource>, bool> getNextPage, string path, object pathParameters, CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken);

            var link = await ResolveLink(cancellationToken).ConfigureAwait(false);
            var parameters = ParameterHelper.CombineParameters(GetAdditionalQueryParameters(), pathParameters);
            await Client.Paginate(path ?? link, parameters, getNextPage, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TResource> FindOne(Func<TResource, bool> search, string path = null, object pathParameters = null)
        {
            return await FindOne(search, path, pathParameters, CancellationToken.None);
        }
        
        public async Task<TResource> FindOne(Func<TResource, bool> search, CancellationToken cancellationToken)
        {
            return await FindOne(search, path: null, pathParameters: null, cancellationToken);
        }
        
        public async Task<TResource> FindOne(Func<TResource, bool> search, string path, object pathParameters, CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken);

            TResource resource = null;
            await Paginate(page =>
                {
                    resource = page.Items.FirstOrDefault(search);
                    return resource == null;
                }, path, pathParameters, cancellationToken)
                .ConfigureAwait(false);
            
            return resource;
        }

        public async Task<List<TResource>> FindMany(Func<TResource, bool> search, string path = null, object pathParameters = null)
        {
            return await FindMany(search, path, pathParameters, CancellationToken.None);
        }
        
        public async Task<List<TResource>> FindMany(Func<TResource, bool> search, CancellationToken cancellationToken)
        {
            return await FindMany(search, path: null, pathParameters: null, cancellationToken);
        }
        
        public async Task<List<TResource>> FindMany(Func<TResource, bool> search, string path, object pathParameters, CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken);

            var resources = new List<TResource>();
            await Paginate(page =>
            {
                resources.AddRange(page.Items.Where(search));
                return true;
            }, path, pathParameters, cancellationToken)
                .ConfigureAwait(false);
            
            return resources;
            
        }

        public Task<List<TResource>> FindAll(string path = null, object pathParameters = null)
        {
            return FindAll(path, pathParameters, CancellationToken.None);
        }
        
        public Task<List<TResource>> FindAll(CancellationToken cancellationToken)
        {
            return FindAll(path: null, pathParameters: null, cancellationToken);
        }
        
        public async Task<List<TResource>> FindAll(string path, object pathParameters, CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken).ConfigureAwait(false);

            return await FindMany(r => true, path, pathParameters, cancellationToken);
        }

        public async Task<List<TResource>> GetAll()
        {
            return await GetAll(CancellationToken.None);
        }
        
        public async Task<List<TResource>> GetAll(CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken);

            var link = await ResolveLink(cancellationToken).ConfigureAwait(false);
            var parameters = ParameterHelper.CombineParameters(GetAdditionalQueryParameters(), new { id = IdValueConstant.IdAll });
            return await Client.Get<List<TResource>>(link, parameters, cancellationToken).ConfigureAwait(false);
        }

        public async Task<List<TResource>> FindByPartialName(string partialName, string path, object pathParameters, CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken).ConfigureAwait(false);

            partialName = (partialName ?? string.Empty).Trim();
            if (pathParameters == null)
                pathParameters = new { partialName = partialName};

            return await FindMany(r =>
            {
                var named = r as INamedResource;
                return named != null && named.Name.Contains(partialName, StringComparison.OrdinalIgnoreCase);
            }, path, pathParameters);
        }
        
        public Task<List<TResource>> FindByPartialName(string partialName, CancellationToken cancellationToken)
        {
            return FindByPartialName(partialName, null, null, cancellationToken);
        }

        public Task<TResource> FindByName(string name, string path = null, object pathParameters = null)
        {
            return FindByName(name, path, pathParameters, CancellationToken.None);
        }

        public Task<TResource> FindByName(string name, CancellationToken cancellationToken)
        {
            return FindByName(name, null, null, cancellationToken);
        }
        
        public async Task<TResource> FindByName(string name, string path, object pathParameters, CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken).ConfigureAwait(false);

            name = (name ?? string.Empty).Trim();

            // Some endpoints allow a Name query param which greatly increases efficiency
            pathParameters ??= new { name = name };

            return await FindOne(r =>
            {
                if (r is INamedResource named) return string.Equals((named.Name ?? string.Empty).Trim(), name, StringComparison.OrdinalIgnoreCase);
                return false;
            }, path, pathParameters, cancellationToken);
        }
        
        public Task<List<TResource>> FindByNames(IEnumerable<string> names, string path = null, object pathParameters = null)
        {
            return FindByNames(names, path, pathParameters, CancellationToken.None);
        }
        
        public Task<List<TResource>> FindByNames(IEnumerable<string> names, CancellationToken cancellationToken)
        {
            return FindByNames(names, path: null, pathParameters: null, cancellationToken);
        }
        
        public async Task<List<TResource>> FindByNames(IEnumerable<string> names, string path, object pathParameters, CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken).ConfigureAwait(false);

            var nameSet = new HashSet<string>((names ?? Array.Empty<string>()).Select(n => (n ?? string.Empty).Trim()), StringComparer.OrdinalIgnoreCase);
            return await FindMany(r =>
            {
                if (r is INamedResource named) return nameSet.Contains((named.Name ?? string.Empty).Trim());
                return false;
            }, path, pathParameters, cancellationToken);
        }

        public async Task<TResource> Get(string idOrHref)
        {
            return await Get(idOrHref, CancellationToken.None);
        }
        
        public async Task<TResource> Get(string idOrHref, CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken);

            if (string.IsNullOrWhiteSpace(idOrHref))
                return null;

            var link = await ResolveLink(cancellationToken).ConfigureAwait(false);
            var additionalQueryParameters = GetAdditionalQueryParameters();
            var parameters = ParameterHelper.CombineParameters(additionalQueryParameters, new { id = idOrHref });
            var  getTask = idOrHref.StartsWith("/", StringComparison.OrdinalIgnoreCase)
                ? Client.Get<TResource>(idOrHref, additionalQueryParameters, cancellationToken).ConfigureAwait(false)
                : Client.Get<TResource>(link, parameters, cancellationToken).ConfigureAwait(false);
            
            return await getTask;
        }

        public virtual async Task<List<TResource>> Get(params string[] ids)
        {
            return await Get(CancellationToken.None, ids);
        }
        
        public virtual async Task<List<TResource>> Get(CancellationToken cancellationToken, params string[] ids)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken);

            if (ids == null) return new List<TResource>();
            var actualIds = ids.Where(id => !string.IsNullOrWhiteSpace(id)).ToArray();
            if (actualIds.Length == 0) return new List<TResource>();

            var resources = new List<TResource>();

            var link = await ResolveLink(cancellationToken).ConfigureAwait(false);
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
                    },
                    cancellationToken
                )
                .ConfigureAwait(false);

            return resources;
        }

        public Task<TResource> Refresh(TResource resource)
        {
            return Refresh(resource, CancellationToken.None);
        }
        
        public async Task<TResource> Refresh(TResource resource, CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken).ConfigureAwait(false);

            if (resource == null) throw new ArgumentNullException("resource");
            return await Get(resource.Id, cancellationToken);
        }

        protected virtual async void EnrichSpaceId(TResource resource)
        {
            await ThrowIfServerVersionIsNotCompatible(CancellationToken.None).ConfigureAwait(false);

            if (resource is IHaveSpaceResource spaceResource)
            {
                spaceResource.SpaceId = Repository.Scope.Apply(space => space.Id,
                    () => null,
                    () => spaceResource.SpaceId);
            }
        }

        protected async Task<string> ResolveLink(CancellationToken cancellationToken)
        {
            await ThrowIfServerVersionIsNotCompatible(cancellationToken);

            if (CollectionLinkName == null && getCollectionLinkName != null)
                CollectionLinkName = await getCollectionLinkName(Repository).ConfigureAwait(false);
            
            // TODO: Add cancellation token support to IOctopusCommonAsyncRepository
            return await Repository.Link(CollectionLinkName).ConfigureAwait(false);
        }
    }

    // ReSharper restore MemberCanBePrivate.Local
    // ReSharper restore UnusedMember.Local
    // ReSharper restore MemberCanBeProtected.Local
}
