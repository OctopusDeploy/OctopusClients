using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IReleaseRepository : IGet<ReleaseResource>, ICreate<ReleaseResource>, IPaginate<ReleaseResource>, IModify<ReleaseResource>, IDelete<ReleaseResource>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="release"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.15)</param>
        /// <returns></returns>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<DeploymentResource>> GetDeployments(ReleaseResource release, int skip = 0, int? take = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="release"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.15)</param>
        /// <returns></returns>
        Task<ResourceCollection<DeploymentResource>> GetDeployments(ReleaseResource release, CancellationToken cancellationToken, int skip = 0, int? take = null);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="release"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.15)</param>
        /// <returns></returns>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<ArtifactResource>> GetArtifacts(ReleaseResource release, int skip = 0, int? take = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="release"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.15)</param>
        /// <returns></returns>
        Task<ResourceCollection<ArtifactResource>> GetArtifacts(ReleaseResource release, CancellationToken cancellationToken, int skip = 0, int? take = null);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<DeploymentTemplateResource> GetTemplate(ReleaseResource release);
        
        Task<DeploymentTemplateResource> GetTemplate(ReleaseResource release, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<DeploymentPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget);
        
        Task<DeploymentPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ReleaseResource> SnapshotVariables(ReleaseResource release);    
        
        Task<ReleaseResource> SnapshotVariables(ReleaseResource release, CancellationToken cancellationToken);    
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ReleaseResource> Create(ReleaseResource release, bool ignoreChannelRules = false);
        
        Task<ReleaseResource> Create(ReleaseResource release, CancellationToken cancellationToken, bool ignoreChannelRules = false);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<LifecycleProgressionResource> GetProgression(ReleaseResource release);
        
        Task<LifecycleProgressionResource> GetProgression(ReleaseResource release, CancellationToken cancellationToken);
    }

    class ReleaseRepository : BasicRepository<ReleaseResource>, IReleaseRepository
    {
        public ReleaseRepository(IOctopusAsyncRepository repository)
            : base(repository, "Releases")
        {
        }

        public Task<ResourceCollection<DeploymentResource>> GetDeployments(ReleaseResource release, int skip = 0, int? take = null)
        {
            return GetDeployments(release, CancellationToken.None, skip: skip, take: take);
        }
        
        public Task<ResourceCollection<DeploymentResource>> GetDeployments(ReleaseResource release, CancellationToken cancellationToken, int skip = 0, int? take = null)
        {
            return Client.List<DeploymentResource>(release.Link("Deployments"), cancellationToken, new { skip, take });
        }

        public Task<ResourceCollection<ArtifactResource>> GetArtifacts(ReleaseResource release, int skip = 0, int? take = null)
        {
            return GetArtifacts(release, CancellationToken.None, skip: skip, take: take);
        }

        public Task<ResourceCollection<ArtifactResource>> GetArtifacts(ReleaseResource release, CancellationToken cancellationToken, int skip = 0, int? take = null)
        {
            return Client.List<ArtifactResource>(release.Link("Artifacts"), cancellationToken, new { skip, take });
        }

        public Task<DeploymentTemplateResource> GetTemplate(ReleaseResource release)
        {
            return GetTemplate(release, CancellationToken.None);
        }

        public Task<DeploymentTemplateResource> GetTemplate(ReleaseResource release, CancellationToken cancellationToken)
        {
            return Client.Get<DeploymentTemplateResource>(release.Link("DeploymentTemplate"), cancellationToken);
        }

        public Task<DeploymentPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget)
        {
            return GetPreview(promotionTarget, CancellationToken.None);
        }
        
        public Task<DeploymentPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget, CancellationToken cancellationToken)
        {
            return Client.Get<DeploymentPreviewResource>(promotionTarget.Link("Preview"), cancellationToken);
        }
        
        public async Task<ReleaseResource> SnapshotVariables(ReleaseResource release)
        {
            return await SnapshotVariables(release, CancellationToken.None);
        }
        
        public async Task<ReleaseResource> SnapshotVariables(ReleaseResource release, CancellationToken cancellationToken)
        {
            await Client.Post(release.Link("SnapshotVariables"), cancellationToken).ConfigureAwait(false);
            return await Get(release.Id).ConfigureAwait(false);
        }

        public async Task<ReleaseResource> Create(ReleaseResource release, bool ignoreChannelRules = false)
        {
            return await Create(release, CancellationToken.None, ignoreChannelRules);
        }
        
        public async Task<ReleaseResource> Create(ReleaseResource release, CancellationToken cancellationToken, bool ignoreChannelRules = false)
        {
            return await Client.Create(await Repository.Link(CollectionLinkName).ConfigureAwait(false), release, cancellationToken, new { ignoreChannelRules }).ConfigureAwait(false);
        }
        
        public Task<LifecycleProgressionResource> GetProgression(ReleaseResource release)
        {
            return GetProgression(release, CancellationToken.None);
        }
        
        public Task<LifecycleProgressionResource> GetProgression(ReleaseResource release, CancellationToken cancellationToken)
        {
            return Client.Get<LifecycleProgressionResource>(release.Links["Progression"], cancellationToken);
        }
    }
}
