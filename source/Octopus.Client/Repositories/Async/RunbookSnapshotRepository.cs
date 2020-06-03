using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IRunbookSnapshotRepository :  IFindByName<RunbookSnapshotResource>, IGet<RunbookSnapshotResource>, ICreate<RunbookSnapshotResource>, IPaginate<RunbookSnapshotResource>, IModify<RunbookSnapshotResource>, IDelete<RunbookSnapshotResource>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="runbookSnapshot"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.15)</param>
        /// <returns></returns>
        Task<ResourceCollection<RunbookRunResource>> GetRunbookRuns(RunbookSnapshotResource runbookSnapshot, int skip = 0, int? take = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="runbookSnapshot"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.15)</param>
        /// <returns></returns>
        Task<ResourceCollection<ArtifactResource>> GetArtifacts(RunbookSnapshotResource runbookSnapshot, int skip = 0, int? take = null);
        Task<RunbookRunTemplateResource> GetTemplate(RunbookSnapshotResource runbookSnapshot);
        Task<RunbookRunPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget);
        Task<RunbookSnapshotResource> SnapshotVariables(RunbookSnapshotResource runbookSnapshot);    
        Task<RunbookSnapshotResource> Create(RunbookSnapshotResource runbookSnapshot);
    }

    class RunbookSnapshotRepository : BasicRepository<RunbookSnapshotResource>, IRunbookSnapshotRepository
    {
        public RunbookSnapshotRepository(IOctopusAsyncRepository repository)
            : base(repository, "RunbookSnapshots")
        {
        }

        public Task<ResourceCollection<RunbookRunResource>> GetRunbookRuns(RunbookSnapshotResource runbookSnapshot, int skip = 0, int? take = null)
        {
            return Client.List<RunbookRunResource>(runbookSnapshot.Link("RunbookRuns"), new { skip, take });
        }

        public Task<ResourceCollection<ArtifactResource>> GetArtifacts(RunbookSnapshotResource runbookSnapshot, int skip = 0, int? take = null)
        {
            return Client.List<ArtifactResource>(runbookSnapshot.Link("Artifacts"), new { skip, take });
        }

        public Task<RunbookRunTemplateResource> GetTemplate(RunbookSnapshotResource runbookSnapshot)
        {
            return Client.Get<RunbookRunTemplateResource>(runbookSnapshot.Link("RunbookRunTemplate"));
        }

        public Task<RunbookRunPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget)
        {
            return Client.Get<RunbookRunPreviewResource>(promotionTarget.Link("RunbookRunPreview"));
        }

        public async Task<RunbookSnapshotResource> SnapshotVariables(RunbookSnapshotResource runbookSnapshot)
        {
            await Client.Post(runbookSnapshot.Link("SnapshotVariables")).ConfigureAwait(false);
            return await Get(runbookSnapshot.Id).ConfigureAwait(false);
        }

        public async Task<RunbookSnapshotResource> Create(RunbookSnapshotResource runbookSnapshot)
        {
            return await Client.Create(await Repository.Link(CollectionLinkName).ConfigureAwait(false), runbookSnapshot).ConfigureAwait(false);
        }
    }
}
