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

        public TResource Get(ProjectResource projectResource, string idOrHref)
        {
            ThrowIfServerVersionIsNotCompatible();

            if (string.IsNullOrWhiteSpace(idOrHref))
                return null;
            var link = projectResource.Link(CollectionLinkName);
            if (idOrHref.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                return Client.Get<TResource>(idOrHref, AdditionalQueryParameters);
            }
            var parameters = ParameterHelper.CombineParameters(AdditionalQueryParameters, new { id = idOrHref });
            return Client.Get<TResource>(link, parameters);
        }

        public  List<TResource> Get(ProjectResource projectResource, params string[] ids)
        {
            ThrowIfServerVersionIsNotCompatible();

            if (ids == null) return new List<TResource>();
            var actualIds = ids.Where(id => !string.IsNullOrWhiteSpace(id)).ToArray();
            if (actualIds.Length == 0) return new List<TResource>();

            var resources = new List<TResource>();
            var link = projectResource.Link(CollectionLinkName);
            if (!Regex.IsMatch(link, @"\{\?.*\Wids\W"))
                link += "{?ids}";
            var parameters = ParameterHelper.CombineParameters(AdditionalQueryParameters, new { ids = actualIds });
            Client.Paginate<TResource>(
                link,
                parameters,
                page =>
                {
                    resources.AddRange(page.Items);
                    return true;
                });

            return resources;
        }
    }

    // ReSharper restore MemberCanBePrivate.Local
    // ReSharper restore UnusedMember.Local
    // ReSharper restore MemberCanBeProtected.Local
}
