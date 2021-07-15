using System;
using System.Threading.Tasks;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;

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

            if (projectResource.PersistenceSettings is VersionControlSettingsResource)
                throw new NotSupportedException($"Version Controlled projects are still in Beta. Use the Beta Repository instead.");

            return await Get(id);
        }
    }

    // ReSharper restore MemberCanBePrivate.Local
    // ReSharper restore UnusedMember.Local
    // ReSharper restore MemberCanBeProtected.Local
}
