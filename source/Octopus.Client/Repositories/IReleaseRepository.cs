using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IReleaseRepository : IGet<ReleaseResource>, ICreate<ReleaseResource>, IPaginate<ReleaseResource>, IModify<ReleaseResource>, IDelete<ReleaseResource>
    {
        Task<ResourceCollection<DeploymentResource>> GetDeployments(ReleaseResource release, int skip = 0);
        Task<ResourceCollection<ArtifactResource>> GetArtifacts(ReleaseResource release, int skip = 0);
        Task<DeploymentTemplateResource> GetTemplate(ReleaseResource release);
        Task<DeploymentPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget);
        Task<ReleaseResource> SnapshotVariables(ReleaseResource release);    
        Task<ReleaseResource> Create(ReleaseResource resource, bool ignoreChannelRules = false);
        Task<ReleaseResource> Modify(ReleaseResource resource, bool ignoreChannelRules = false);
    }
}