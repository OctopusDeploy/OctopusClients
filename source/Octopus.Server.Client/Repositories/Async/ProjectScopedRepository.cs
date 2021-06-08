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

        public async Task<TResource> Get(ProjectResource projectResource, string id)
        {
            await ThrowIfServerVersionIsNotCompatible();

            if (string.IsNullOrWhiteSpace(id))
                return null;

            var link = $"{projectResource.Link(CollectionLinkName)}/{id}";
            var additionalQueryParameters = GetAdditionalQueryParameters();
            return await Client.Get<TResource>(link, additionalQueryParameters).ConfigureAwait(false);
        }
    }

    // ReSharper restore MemberCanBePrivate.Local
    // ReSharper restore UnusedMember.Local
    // ReSharper restore MemberCanBeProtected.Local
}
