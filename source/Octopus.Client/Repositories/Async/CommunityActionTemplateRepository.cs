using System.Threading.Tasks;
using Octopus.Client.Model;

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
        public CommunityActionTemplateRepository(IOctopusAsyncClient client) : base(client, "CommunityActionTemplates")
        {
        }

        public Task Install(CommunityActionTemplateResource resource)
        {
            return Client.Post(resource.Links["Installation"]);
        }

        public Task UpdateInstallation(CommunityActionTemplateResource resource)
        {
            return Client.Put(resource.Links["Installation"]);
        }

        public Task<ActionTemplateResource> GetInstalledTemplate(CommunityActionTemplateResource resource)
        {
            return Client.Get<ActionTemplateResource>(resource.Links["InstalledTemplate"]);
        }
    }
}