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
        public CommunityActionTemplateRepository(IOctopusRepository repository) : base(repository, "CommunityActionTemplates")
        {
        }

        public void Install(CommunityActionTemplateResource resource)
        {
            Repository.SpaceContext.EnsureSingleSpaceContext();
            Client.Post(resource.Links["Installation"].AppendSpaceId(Repository.SpaceContext.SpaceIds.Single()));
        }

        public void UpdateInstallation(CommunityActionTemplateResource resource)
        {
            Repository.SpaceContext.EnsureSingleSpaceContext();
            Client.Put(resource.Links["Installation"].AppendSpaceId(Repository.SpaceContext.SpaceIds.Single()));
        }

        public ActionTemplateResource GetInstalledTemplate(CommunityActionTemplateResource resource)
        {
            Repository.SpaceContext.EnsureSingleSpaceContext();
            return Client.Get<ActionTemplateResource>(resource.Links["InstalledTemplate"].AppendSpaceId(Repository.SpaceContext.SpaceIds.Single()));
        }
    }
}