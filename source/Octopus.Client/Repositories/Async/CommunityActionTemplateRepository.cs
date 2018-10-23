using System.Linq;
using System.Threading.Tasks;
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
            Repository.SpaceContext.EnsureSingleSpaceContext();
            return Client.Post(resource.Links["Installation"].AppendSpaceId(Repository.SpaceContext.SpaceIds.Single()));
        }

        public Task UpdateInstallation(CommunityActionTemplateResource resource)
        {
            Repository.SpaceContext.EnsureSingleSpaceContext();
            return Client.Put(resource.Links["Installation"].AppendSpaceId(Repository.SpaceContext.SpaceIds.Single()));
        }

        public Task<ActionTemplateResource> GetInstalledTemplate(CommunityActionTemplateResource resource)
        {
            Repository.SpaceContext.EnsureSingleSpaceContext();
            return Client.Get<ActionTemplateResource>(resource.Links["InstalledTemplate"].AppendSpaceId(Repository.SpaceContext.SpaceIds.Single()));
        }
    }
}