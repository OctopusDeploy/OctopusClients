using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ICommunityActionTemplateRepository : IGet<CommunityActionTemplateResource>
    {
        ActionTemplateResource GetInstalledTemplate(CommunityActionTemplateResource resource);
        void Install(CommunityActionTemplateResource resource);
        void UpdateInstallation(CommunityActionTemplateResource resource);
    }
    
    class CommunityActionTemplateRepository : BasicRepository<CommunityActionTemplateResource>, ICommunityActionTemplateRepository
    {
        public CommunityActionTemplateRepository(IOctopusClient client) : base(client, "CommunityActionTemplates")
        {
        }

        public void Install(CommunityActionTemplateResource resource)
        {
            Client.Post(resource.Links["Installation"]);
        }

        public void UpdateInstallation(CommunityActionTemplateResource resource)
        {
            Client.Put(resource.Links["Installation"]);
        }

        public ActionTemplateResource GetInstalledTemplate(CommunityActionTemplateResource resource)
        {
            return Client.Get<ActionTemplateResource>(resource.Links["InstalledTemplate"]);
        }
    }
}