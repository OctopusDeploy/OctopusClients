using System;
using System.Linq;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;
using Octopus.Client.Util;

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
            Client.SpaceContext.EnsureSingleSpaceContext();
            Client.Post(resource.Links["Installation"].AppendSpaceId(Client.SpaceContext.SpaceIds.Single()));
        }

        public void UpdateInstallation(CommunityActionTemplateResource resource)
        {
            Client.SpaceContext.EnsureSingleSpaceContext();
            Client.Put(resource.Links["Installation"].AppendSpaceId(Client.SpaceContext.SpaceIds.Single()));
        }

        public ActionTemplateResource GetInstalledTemplate(CommunityActionTemplateResource resource)
        {
            Client.SpaceContext.EnsureSingleSpaceContext();
            return Client.Get<ActionTemplateResource>(resource.Links["InstalledTemplate"].AppendSpaceId(Client.SpaceContext.SpaceIds.Single()));
        }
    }
}