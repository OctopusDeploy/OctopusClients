using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    // ReSharper disable MemberCanBePrivate.Local
    // ReSharper disable UnusedMember.Local
    // ReSharper disable MemberCanBeProtected.Local
    abstract class ProjectScopedRepository<TResource> : BasicRepository<TResource> where TResource : class, IResource, IHaveProject
    {
        protected ProjectScopedRepository(
            IOctopusRepository repository,
            string collectionLinkName,
            Func<IOctopusRepository, string> getCollectionLinkName = null)
        : base(repository, collectionLinkName, getCollectionLinkName)
        {
        }

        public override TResource Create(TResource resource, object pathParameters = null)
        {
            var projectResource = Repository.Projects.Get(resource.ProjectId);
            if (projectResource.PersistenceSettings.Type == PersistenceSettingsType.VersionControlled)
            {
                return Create(projectResource, resource, pathParameters);
            }

            return base.Create(resource, pathParameters);
        }

        public TResource Create(ProjectResource projectResource, TResource resource, object pathParameters = null)
        {
            ThrowIfServerVersionIsNotCompatible();

            var link = projectResource.Link(CollectionLinkName);
            EnrichSpaceId(resource);
            return Client.Create(link, resource, pathParameters);
        }

        public TResource Get(ProjectResource projectResource, string id)
        {
            ThrowIfServerVersionIsNotCompatible();

            if (string.IsNullOrWhiteSpace(id))
                return null;
            var baseUrl = UrlTemplate.Resolve(projectResource.Link(CollectionLinkName), null);
            var link = $"{baseUrl}/{id}";
            return Client.Get<TResource>(link, AdditionalQueryParameters);
        }
    }

    // ReSharper restore MemberCanBePrivate.Local
    // ReSharper restore UnusedMember.Local
    // ReSharper restore MemberCanBeProtected.Local
}
