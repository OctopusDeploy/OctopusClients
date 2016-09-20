using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IReleaseRepository : IGet<ReleaseResource>, ICreate<ReleaseResource>, IPaginate<ReleaseResource>, IModify<ReleaseResource>, IDelete<ReleaseResource>
    {
        ResourceCollection<DeploymentResource> GetDeployments(ReleaseResource release, int skip = 0);
        ResourceCollection<ArtifactResource> GetArtifacts(ReleaseResource release, int skip = 0);
        DeploymentTemplateResource GetTemplate(ReleaseResource release);
        DeploymentPreviewResource GetPreview(DeploymentPromotionTarget promotionTarget);
        ReleaseResource SnapshotVariables(ReleaseResource release);    
        ReleaseResource Create(ReleaseResource resource, bool ignoreChannelRules = false);
        ReleaseResource Modify(ReleaseResource resource, bool ignoreChannelRules = false);
    }
}