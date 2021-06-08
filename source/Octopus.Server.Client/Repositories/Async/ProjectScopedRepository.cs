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
    abstract class ProjectScopedRepository<TResource> : BasicRepository<TResource> where TResource : class, IResource, IHaveProject
    {
        protected ProjectScopedRepository(
            IOctopusAsyncRepository repository,
            string collectionLinkName,
            Func<IOctopusAsyncRepository, Task<string>> getCollectionLinkName = null)
        : base(repository, collectionLinkName, getCollectionLinkName)
        {
        }

        public override async Task<TResource> Create(TResource resource, object pathParameters = null)
        {
            var projectResource = await Repository.Projects.Get(resource.ProjectId);
            if (projectResource.PersistenceSettings.Type == PersistenceSettingsType.VersionControlled)
            {
                return await Create(projectResource, resource, pathParameters);
            }

            return await base.Create(resource, pathParameters);
        }

        public async Task<TResource> Create(ProjectResource projectResource, TResource resource, object pathParameters = null)
        {
            await ThrowIfServerVersionIsNotCompatible();

            var link = projectResource.Link(CollectionLinkName);
            EnrichSpaceId(resource);
            return await Client.Create(link, resource, pathParameters).ConfigureAwait(false);
        }

        public async Task<TResource> Get(ProjectResource projectResource, string idOrHref)
        {
            await ThrowIfServerVersionIsNotCompatible();

            if (string.IsNullOrWhiteSpace(idOrHref))
                return null;

            var link = projectResource.Link(CollectionLinkName);
            var additionalQueryParameters = GetAdditionalQueryParameters();
            var parameters = ParameterHelper.CombineParameters(additionalQueryParameters, new { id = idOrHref });
            var  getTask = idOrHref.StartsWith("/", StringComparison.OrdinalIgnoreCase)
                ? Client.Get<TResource>(idOrHref, additionalQueryParameters).ConfigureAwait(false)
                : Client.Get<TResource>(link, parameters).ConfigureAwait(false);
            return await getTask;
        }

        public async Task<List<TResource>> Get(ProjectResource projectResource, params string[] ids)
        {
            await ThrowIfServerVersionIsNotCompatible();

            if (ids == null) return new List<TResource>();
            var actualIds = ids.Where(id => !string.IsNullOrWhiteSpace(id)).ToArray();
            if (actualIds.Length == 0) return new List<TResource>();

            var resources = new List<TResource>();

            var link = projectResource.Link(CollectionLinkName);
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
    }

    // ReSharper restore MemberCanBePrivate.Local
    // ReSharper restore UnusedMember.Local
    // ReSharper restore MemberCanBeProtected.Local
}
