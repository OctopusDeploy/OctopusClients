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
            var link = Repository.Scope.Apply(space => baseLink.AppendSpaceId(space),
                () => throw new SpaceScopedOperationInSystemContextException(),
                () => baseLink.ToString()); // Link without a space id acts on the default space
            return Client.Post(link);
        }

        public Task UpdateInstallation(CommunityActionTemplateResource resource)
        {
            var baseLink = resource.Links["Installation"];
            var link = Repository.Scope.Apply(space => baseLink.AppendSpaceId(space),
                () => throw new SpaceScopedOperationInSystemContextException(),
                () => baseLink.ToString()); // Link without a space id acts on the default space
            return Client.Put(link);
        }

        public Task<ActionTemplateResource> GetInstalledTemplate(CommunityActionTemplateResource resource)
        {
            var baseLink = resource.Links["InstalledTemplate"];
            var link = Repository.Scope.Apply(space => baseLink.AppendSpaceId(space),
                () => throw new SpaceScopedOperationInSystemContextException(),
                () => baseLink.ToString()); // Link without a space id acts on the default space
            return Client.Get<ActionTemplateResource>(link);
        }
    }
}