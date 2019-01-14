using System;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories.Async
{
    public interface ICommunityActionTemplateRepository : IGet<CommunityActionTemplateResource>
    {
        Task<ActionTemplateResource> GetInstalledTemplate(CommunityActionTemplateResource resource);
        Task Install(CommunityActionTemplateResource resource);
        Task UpdateInstallation(CommunityActionTemplateResource resource);
    }

    class CommunityActionTemplateRepository : BasicRepository<CommunityActionTemplateResource>, ICommunityActionTemplateRepository
    {
        public CommunityActionTemplateRepository(IOctopusAsyncRepository repository) : base(repository, "CommunityActionTemplates")
        {
        }

        public Task Install(CommunityActionTemplateResource resource)
        {
            var baseLink = resource.Links["Installation"];
            var spaceResource = Repository.Scope.Apply(space => space,
                () => throw new SpaceScopedOperationInSystemContextException(),
                () => null); // Link without a space id acts on the default space

            if (spaceResource == null)
            {
                return Client.Post(baseLink.ToString());
            }

            return Client.Post<string>(baseLink.ToString(), null, new {spaceId = spaceResource.Id});
        }

        public Task UpdateInstallation(CommunityActionTemplateResource resource)
        {
            var baseLink = resource.Links["Installation"];
            var spaceResource = Repository.Scope.Apply(space => space,
                () => throw new SpaceScopedOperationInSystemContextException(),
                () => null); // Link without a space id acts on the default space

            if (spaceResource == null)
            {
                return Client.Put(baseLink.ToString());
            }

            return Client.Put<string>(baseLink.ToString(), null, new {spaceId = spaceResource.Id});
        }

        public Task<ActionTemplateResource> GetInstalledTemplate(CommunityActionTemplateResource resource)
        {
            var baseLink = resource.Links["InstalledTemplate"];
            var spaceResource = Repository.Scope.Apply(space => space,
                () => throw new SpaceScopedOperationInSystemContextException(),
                () => null); // Link without a space id acts on the default space

            if (spaceResource == null)
            {
                return Client.Get<ActionTemplateResource>(baseLink.ToString());
            }

            return Client.Get<ActionTemplateResource>(baseLink.ToString(), new {spaceId = spaceResource.Id});
        }
    }
}