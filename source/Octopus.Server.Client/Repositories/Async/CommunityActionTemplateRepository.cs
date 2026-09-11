using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories.Async
{
    public interface ICommunityActionTemplateRepository : IGet<CommunityActionTemplateResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ActionTemplateResource> GetInstalledTemplate(CommunityActionTemplateResource resource);
        Task<ActionTemplateResource> GetInstalledTemplate(CommunityActionTemplateResource resource, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task Install(CommunityActionTemplateResource resource);
        Task Install(CommunityActionTemplateResource resource, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task UpdateInstallation(CommunityActionTemplateResource resource);
        Task UpdateInstallation(CommunityActionTemplateResource resource, CancellationToken cancellationToken);
    }

    class CommunityActionTemplateRepository : BasicRepository<CommunityActionTemplateResource>, ICommunityActionTemplateRepository
    {
        public CommunityActionTemplateRepository(IOctopusAsyncRepository repository) : base(repository, "CommunityActionTemplates")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task Install(CommunityActionTemplateResource resource)
            => Install(resource, CancellationToken.None);

        public Task Install(CommunityActionTemplateResource resource, CancellationToken cancellationToken)
        {
            var baseLink = resource.Links["Installation"];
            var spaceResource = Repository.Scope.Apply(space => space,
                () => throw new SpaceScopedOperationInSystemContextException(),
                () => null); // Link without a space id acts on the default space

            if (spaceResource == null)
            {
                return Client.Post(baseLink.ToString(), cancellationToken);
            }

            return Client.Post<string>(baseLink.ToString(), null, new { spaceId = spaceResource.Id }, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task UpdateInstallation(CommunityActionTemplateResource resource)
            => UpdateInstallation(resource, CancellationToken.None);

        public Task UpdateInstallation(CommunityActionTemplateResource resource, CancellationToken cancellationToken)
        {
            var baseLink = resource.Links["Installation"];
            var spaceResource = Repository.Scope.Apply(space => space,
                () => throw new SpaceScopedOperationInSystemContextException(),
                () => null); // Link without a space id acts on the default space

            if (spaceResource == null)
            {
                return Client.Put(baseLink.ToString(), cancellationToken);
            }

            return Client.Put<string>(baseLink.ToString(), null, new { spaceId = spaceResource.Id }, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ActionTemplateResource> GetInstalledTemplate(CommunityActionTemplateResource resource)
            => GetInstalledTemplate(resource, CancellationToken.None);

        public Task<ActionTemplateResource> GetInstalledTemplate(CommunityActionTemplateResource resource, CancellationToken cancellationToken)
        {
            var baseLink = resource.Links["InstalledTemplate"];
            var spaceResource = Repository.Scope.Apply(space => space,
                () => throw new SpaceScopedOperationInSystemContextException(),
                () => null); // Link without a space id acts on the default space

            if (spaceResource == null)
            {
                return Client.Get<ActionTemplateResource>(baseLink.ToString(), cancellationToken);
            }

            return Client.Get<ActionTemplateResource>(baseLink.ToString(), new { spaceId = spaceResource.Id }, cancellationToken);
        }
    }
}
