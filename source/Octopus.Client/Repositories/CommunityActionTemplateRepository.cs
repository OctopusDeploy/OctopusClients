using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;
using Octopus.Client.Util;

namespace Octopus.Client.Repositories
{
    public interface ICommunityActionTemplateRepository : IGet<CommunityActionTemplateResource>
    {
        ActionTemplateResource GetInstalledTemplate(CommunityActionTemplateResource resource, string spaceId = null);
        void Install(CommunityActionTemplateResource resource, string spaceId = null);
        void UpdateInstallation(CommunityActionTemplateResource resource, string spaceId = null);
    }
    
    class CommunityActionTemplateRepository : BasicRepository<CommunityActionTemplateResource>, ICommunityActionTemplateRepository
    {
        public CommunityActionTemplateRepository(IOctopusClient client) : base(client, "CommunityActionTemplates")
        {
        }

        public void Install(CommunityActionTemplateResource resource, string spaceId = null)
        {
            Client.Post(resource.Links["Installation"].AppendSpaceId(spaceId));
        }

        public void UpdateInstallation(CommunityActionTemplateResource resource, string spaceId = null)
        {
            Client.Put(resource.Links["Installation"].AppendSpaceId(spaceId));
        }

        public ActionTemplateResource GetInstalledTemplate(CommunityActionTemplateResource resource, string spaceId = null)
        {
            return Client.Get<ActionTemplateResource>(resource.Links["InstalledTemplate"].AppendSpaceId(spaceId));
        }
    }
}