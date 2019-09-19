using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Model.OpsProcesses;

namespace Octopus.Client.Repositories.Async
{
    public interface IOpsSnapshotRepository : IGet<OpsSnapshotResource>, ICreate<OpsSnapshotResource>, IPaginate<OpsSnapshotResource>, IModify<OpsSnapshotResource>, IDelete<OpsSnapshotResource>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="opsSnapshot"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.15)</param>
        /// <returns></returns>
        Task<ResourceCollection<OpsRunResource>> GetOpsRuns(OpsSnapshotResource opsSnapshot, int skip = 0, int? take = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="opsSnapshot"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.15)</param>
        /// <returns></returns>
        Task<ResourceCollection<ArtifactResource>> GetArtifacts(OpsSnapshotResource opsSnapshot, int skip = 0, int? take = null);
        Task<OpsRunTemplateResource> GetTemplate(OpsSnapshotResource opsSnapshot);
        Task<OpsRunPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget);
        Task<OpsSnapshotResource> SnapshotVariables(OpsSnapshotResource opsSnapshot);    
        Task<OpsSnapshotResource> Create(OpsSnapshotResource opsSnapshot);
    }

    class OpsSnapshotRepository : BasicRepository<OpsSnapshotResource>, IOpsSnapshotRepository
    {
        public OpsSnapshotRepository(IOctopusAsyncRepository repository)
            : base(repository, "OpsSnapshots")
        {
        }

        public Task<ResourceCollection<OpsRunResource>> GetOpsRuns(OpsSnapshotResource opsSnapshot, int skip = 0, int? take = null)
        {
            return Client.List<OpsRunResource>(opsSnapshot.Link("OpsRuns"), new { skip, take });
        }

        public Task<ResourceCollection<ArtifactResource>> GetArtifacts(OpsSnapshotResource opsSnapshot, int skip = 0, int? take = null)
        {
            return Client.List<ArtifactResource>(opsSnapshot.Link("Artifacts"), new { skip, take });
        }

        public Task<OpsRunTemplateResource> GetTemplate(OpsSnapshotResource opsSnapshot)
        {
            return Client.Get<OpsRunTemplateResource>(opsSnapshot.Link("OpsRunTemplate"));
        }

        public Task<OpsRunPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget)
        {
            return Client.Get<OpsRunPreviewResource>(promotionTarget.Link("OpsRunPreview"));
        }

        public async Task<OpsSnapshotResource> SnapshotVariables(OpsSnapshotResource opsSnapshot)
        {
            await Client.Post(opsSnapshot.Link("SnapshotVariables")).ConfigureAwait(false);
            return await Get(opsSnapshot.Id).ConfigureAwait(false);
        }

        public async Task<OpsSnapshotResource> Create(OpsSnapshotResource opsSnapshot)
        {
            return await Client.Create(await Repository.Link(CollectionLinkName).ConfigureAwait(false), opsSnapshot).ConfigureAwait(false);
        }
    }
}
