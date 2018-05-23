using System.Threading.Tasks;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ICommunityActionTemplateRepository : IGet<CommunityActionTemplateResource>
    {
        Task<ActionTemplateResource> GetInstalledTemplate(CommunityActionTemplateResource resource, string spaceId = null);
        Task Install(CommunityActionTemplateResource resource, string spaceId = null);
        Task UpdateInstallation(CommunityActionTemplateResource resource, string spaceId = null);
    }

    class CommunityActionTemplateRepository : BasicRepository<CommunityActionTemplateResource>, ICommunityActionTemplateRepository
    {
        public CommunityActionTemplateRepository(IOctopusAsyncClient client) : base(client, "CommunityActionTemplates")
        {
        }

        public Task Install(CommunityActionTemplateResource resource, string spaceId = null)
        {
            return Client.Post(resource.Links["Installation"].AppendSpaceId(spaceId));
        }

        public Task UpdateInstallation(CommunityActionTemplateResource resource, string spaceId = null)
        {
            return Client.Put(resource.Links["Installation"].AppendSpaceId(spaceId));
        }

        public Task<ActionTemplateResource> GetInstalledTemplate(CommunityActionTemplateResource resource, string spaceId = null)
        {
            return Client.Get<ActionTemplateResource>(resource.Links["InstalledTemplate"].AppendSpaceId(spaceId));
        }
    }

    static class LinkSpaceExtension
    {
        public static string AppendSpaceId(this Href link, string spaceId)
        {
            if (!string.IsNullOrEmpty(spaceId))
            {
                link += $"/{spaceId}";
            }
            return link;
        }
    }
}