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
        public CommunityActionTemplateRepository(IOctopusRepository repository) : base(repository, "CommunityActionTemplates")
        {
        }

        public void Install(CommunityActionTemplateResource resource)
        {
            var baseLink = resource.Links["Installation"];
            
            var spaceResource = Repository.Scope.Apply(space => space,
                () => throw new SpaceScopedOperationInSystemContextException(),
                () => null); // Link without a space id acts on the default space

            if (spaceResource == null)
            {
                Client.Post(baseLink.ToString());
            }
            else
            {
                Client.Post<string>(baseLink.ToString(), null, new {spaceId = spaceResource.Id});
            }
        }

        public void UpdateInstallation(CommunityActionTemplateResource resource)
        {
            var baseLink = resource.Links["Installation"];
            var spaceResource = Repository.Scope.Apply(space => space,
                () => throw new SpaceScopedOperationInSystemContextException(),
                () => null); // Link without a space id acts on the default space

            if (spaceResource == null)
            {
                Client.Put(baseLink.ToString());
            }
            else
            {
                Client.Put<string>(baseLink.ToString(), null, new {spaceId = spaceResource.Id});
            }
        }

        public ActionTemplateResource GetInstalledTemplate(CommunityActionTemplateResource resource)
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
