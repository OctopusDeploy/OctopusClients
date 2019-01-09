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
            var baseLink = resource.Links["Installation"];
            var link = Repository.Scope.Apply(space => baseLink.AppendSpaceId(space.Id),
                () => throw new SpaceScopedOperationInSystemContextException(),
                () => baseLink.ToString()); // Link without a space id acts on the default space
            Client.Post(link);
        }

        public void UpdateInstallation(CommunityActionTemplateResource resource)
        {
            var baseLink = resource.Links["Installation"];
            var link = Repository.Scope.Apply(space => baseLink.AppendSpaceId(space.Id),
                () => throw new SpaceScopedOperationInSystemContextException(),
                () => baseLink.ToString()); // Link without a space id acts on the default space
            Client.Put(link);
        }

        public ActionTemplateResource GetInstalledTemplate(CommunityActionTemplateResource resource)
        {
            var baseLink = resource.Links["InstalledTemplate"];
            var link = Repository.Scope.Apply(space => baseLink.AppendSpaceId(space.Id),
                () => throw new SpaceScopedOperationInSystemContextException(),
                () => baseLink.ToString()); // Link without a space id acts on the default space
            return Client.Get<ActionTemplateResource>(link);
        }
    }
}