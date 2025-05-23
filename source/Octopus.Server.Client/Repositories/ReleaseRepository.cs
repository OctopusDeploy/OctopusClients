using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IReleaseRepository : IGet<ReleaseResource>, ICreate<ReleaseResource>, IPaginate<ReleaseResource>, IModify<ReleaseResource>, IDelete<ReleaseResource>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="release"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.159)</param>
        /// <returns></returns>
        ResourceCollection<DeploymentResource> GetDeployments(ReleaseResource release, int skip = 0, int? take = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="release"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.159)</param>
        /// <returns></returns>
        ResourceCollection<ArtifactResource> GetArtifacts(ReleaseResource release, int skip = 0, int? take = null);
        DeploymentTemplateResource GetTemplate(ReleaseResource release);
        DeploymentPreviewResource GetPreview(DeploymentPromotionTarget promotionTarget);
        ReleaseResource SnapshotVariables(ReleaseResource release);    
        ReleaseResource Create(ReleaseResource release, bool ignoreChannelRules = false);
        LifecycleProgressionResource GetProgression(ReleaseResource release);
        GetMissingPackagesForReleaseResponse GetMissingPackagesForRelease(GetMissingPackagesForReleaseRequest request);
    }
    
    class ReleaseRepository : BasicRepository<ReleaseResource>, IReleaseRepository
    {
        public ReleaseRepository(IOctopusRepository repository)
            : base(repository, "Releases")
        {
        }

        public ResourceCollection<DeploymentResource> GetDeployments(ReleaseResource release, int skip = 0, int? take = null)
        {
            return Client.List<DeploymentResource>(release.Link("Deployments"), new { skip, take });
        }

        public ResourceCollection<ArtifactResource> GetArtifacts(ReleaseResource release, int skip = 0, int? take = null)
        {
            return Client.List<ArtifactResource>(release.Link("Artifacts"), new { skip, take });
        }

        public DeploymentTemplateResource GetTemplate(ReleaseResource release)
        {
            return Client.Get<DeploymentTemplateResource>(release.Link("DeploymentTemplate"));
        }

        public DeploymentPreviewResource GetPreview(DeploymentPromotionTarget promotionTarget)
        {
            return Client.Get<DeploymentPreviewResource>(promotionTarget.Link("Preview"));
        }

        public ReleaseResource SnapshotVariables(ReleaseResource release)
        {
            Client.Post(release.Link("SnapshotVariables"));
            return Get(release.Id);
        }

        public ReleaseResource Create(ReleaseResource release, bool ignoreChannelRules = false)
        {
            return Client.Create(Repository.Link(CollectionLinkName), release, new { ignoreChannelRules });
        }

        public LifecycleProgressionResource GetProgression(ReleaseResource release)
        {
            return Client.Get<LifecycleProgressionResource>(release.Links["Progression"]);
        }

        public GetMissingPackagesForReleaseResponse GetMissingPackagesForRelease(
            GetMissingPackagesForReleaseRequest request)
        {
            const string link  = "/api/{spaceId}/releases/{releaseId}/missingpackages";
            return Client.Get<GetMissingPackagesForReleaseResponse>(link, new { request.SpaceId, request.ReleaseId });
        }
    }
}