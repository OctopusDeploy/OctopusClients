﻿using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Model;
using System.Text.RegularExpressions;
using Octopus.Client.Exceptions;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensions;
using Octopus.Client.Util;
using Octopus.Client.Validation;

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
        private SemanticVersion minimumRequiredVersion;
        private bool hasMinimumRequiredVersion;
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

                    if (!isDefaultSpaceFound && ServerSupportsSpaces())
                    {
                        throw new DefaultSpaceNotFoundException(spaceResource);
                    }
                });
        }

        private bool ServerSupportsSpaces()
        {
            var rootDocument = Repository.LoadRootDocument();

            var spacesIsSupported = rootDocument.HasLink("Spaces");

            return spacesIsSupported;
        }

        protected void MinimumCompatibleVersion(string version)
        {
            minimumRequiredVersion = SemanticVersion.Parse(version);
            hasMinimumRequiredVersion = true;
        }

        protected void ThrowIfServerVersionIsNotCompatible()
        {
            if (!hasMinimumRequiredVersion) return;

            EnsureServerIsMinimumVersion(minimumRequiredVersion, currentServerVersion => 
                    $"The version of the Octopus Server ('{currentServerVersion}') you are connecting to is not compatible with this version of Octopus.Client for this API call. Please upgrade your Octopus Server to a version greater than '{minimumRequiredVersion}'");
        }
        
        protected void EnsureServerIsMinimumVersion(SemanticVersion requiredVersion, Func<string, string> messageGenerator)
        {
            var currentServerVersion = Repository.LoadRootDocument().Version;

            if (ServerVersionCheck.IsOlderThanClient(currentServerVersion, requiredVersion))
            {
                throw new NotSupportedException(messageGenerator(currentServerVersion));
            }
        }

        private void AssertSpaceIdMatchesResource(TResource resource)
        {
            ThrowIfServerVersionIsNotCompatible();

            if (resource is IHaveSpaceResource spaceResource)
                CheckSpaceResource(spaceResource);
        }

        public virtual TResource Create(TResource resource, object pathParameters = null)
        {
            ThrowIfServerVersionIsNotCompatible();

            if (resource == null) throw new ArgumentNullException(nameof(resource));
            AssertSpaceIdMatchesResource(resource);

            var link = ResolveLink();
            EnrichSpaceId(resource);
            return client.Create(link, resource, pathParameters);
        }

        public virtual TResource Modify(TResource resource)
        {
            ThrowIfServerVersionIsNotCompatible();

            if (resource == null) throw new ArgumentNullException(nameof(resource));
            AssertSpaceIdMatchesResource(resource);
            return client.Update(resource.Links["Self"], resource);
        }

        public void Delete(TResource resource)
        {
            ThrowIfServerVersionIsNotCompatible();

            if (resource == null) throw new ArgumentNullException(nameof(resource));
            AssertSpaceIdMatchesResource(resource);
            client.Delete(resource.Links["Self"]);
        }

        public void Paginate(Func<ResourceCollection<TResource>, bool> getNextPage, string path = null, object pathParameters = null)
        {
            ThrowIfServerVersionIsNotCompatible();

            var link = ResolveLink();
            var parameters = ParameterHelper.CombineParameters(AdditionalQueryParameters, pathParameters);
            client.Paginate(path ?? link, parameters, getNextPage);
        }

        public TResource FindOne(Func<TResource, bool> search, string path = null, object pathParameters = null)
        {
            ThrowIfServerVersionIsNotCompatible();

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
            ThrowIfServerVersionIsNotCompatible();

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
            ThrowIfServerVersionIsNotCompatible();

            return FindMany(r => true, path, pathParameters);
        }

        public List<TResource> GetAll()
        {
            ThrowIfServerVersionIsNotCompatible();

            var link = ResolveLink();
            var parameters = ParameterHelper.CombineParameters(AdditionalQueryParameters, new { id = IdValueConstant.IdAll });
            return client.Get<List<TResource>>(link, parameters);
        }

        public List<TResource> FindByPartialName(string partialName, string path = null, object pathParameters = null)
        {
            ThrowIfServerVersionIsNotCompatible();

            partialName = (partialName ?? string.Empty).Trim();
            if (pathParameters == null)
                pathParameters = new { partialName = partialName};

            return FindMany(r =>
            {
                var named = r as INamedResource;
                return named != null && named.Name.Contains(partialName, StringComparison.OrdinalIgnoreCase);
            }, path, pathParameters);
        }

        public TResource FindByName(string name, string path = null, object pathParameters = null)
        {
            ThrowIfServerVersionIsNotCompatible();

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
            ThrowIfServerVersionIsNotCompatible();

            var nameSet = new HashSet<string>((names ?? new string[0]).Select(n => (n ?? string.Empty).Trim()), StringComparer.OrdinalIgnoreCase);
            return FindMany(r =>
            {
                var named = r as INamedResource;
                if (named != null) return nameSet.Contains((named.Name ?? string.Empty).Trim());
                return false;
            }, path, pathParameters);
        }
        
        public TResource FindBySlug(string slug, string path = null, object pathParameters = null)
        {
            ThrowIfServerVersionIsNotCompatible();

            slug = (slug ?? string.Empty).Trim();
            // Some endpoints allow a Slug query param which greatly increases efficiency
            if (pathParameters == null)
                pathParameters = new {slug = slug};

            return FindOne(r =>
            {
                var slugd = r as IHaveSlugResource;
                if (slugd != null) return string.Equals((slugd.Slug ?? string.Empty).Trim(), slug, StringComparison.OrdinalIgnoreCase);
                return false;
            }, path, pathParameters);
        }

        public List<TResource> FindBySlugs(IEnumerable<string> slugs, string path = null, object pathParameters = null)
        {
            ThrowIfServerVersionIsNotCompatible();

            var slugSet = new HashSet<string>((slugs ?? new string[0]).Select(n => (n ?? string.Empty).Trim()), StringComparer.OrdinalIgnoreCase);
            return FindMany(r =>
            {
                var slugd = r as IHaveSlugResource;
                if (slugd != null) return slugSet.Contains((slugd.Slug ?? string.Empty).Trim());
                return false;
            }, path, pathParameters);
        }

        public TResource Get(string idOrHref)
        {
            ThrowIfServerVersionIsNotCompatible();

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
            ThrowIfServerVersionIsNotCompatible();

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
            ThrowIfServerVersionIsNotCompatible();

            if (resource == null) throw new ArgumentNullException("resource");
            return Get(resource.Id);
        }

        protected virtual void EnrichSpaceId(TResource resource)
        {
            ThrowIfServerVersionIsNotCompatible();

            if (resource is IHaveSpaceResource spaceResource)
            {
                spaceResource.SpaceId = Repository.Scope.Apply(space => space.Id,
                    () => null,
                    () => spaceResource.SpaceId);
            }
        }

        protected string ResolveLink()
        {
            ThrowIfServerVersionIsNotCompatible();

            if (CollectionLinkName == null && getCollectionLinkName != null)
                CollectionLinkName = getCollectionLinkName(Repository);
            return Repository.Link(CollectionLinkName);
        }
    }

    // ReSharper restore MemberCanBePrivate.Local
    // ReSharper restore UnusedMember.Local
    // ReSharper restore MemberCanBeProtected.Local
}