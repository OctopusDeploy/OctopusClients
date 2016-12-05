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
}