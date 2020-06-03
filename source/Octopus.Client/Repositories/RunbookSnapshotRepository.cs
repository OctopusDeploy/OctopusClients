using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IRunbookSnapshotRepository : IFindByName<RunbookSnapshotResource>, IGet<RunbookSnapshotResource>, ICreate<RunbookSnapshotResource>, IPaginate<RunbookSnapshotResource>, IModify<RunbookSnapshotResource>, IDelete<RunbookSnapshotResource>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="runbookSnapshot"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.159)</param>
        /// <returns></returns>
        ResourceCollection<RunbookRunResource> GetRunbookRuns(RunbookSnapshotResource runbookSnapshot, int skip = 0, int? take = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="runbookSnapshot"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.159)</param>
        /// <returns></returns>
        ResourceCollection<ArtifactResource> GetArtifacts(RunbookSnapshotResource runbookSnapshot, int skip = 0, int? take = null);
        RunbookRunTemplateResource GetTemplate(RunbookSnapshotResource runbookSnapshot);
        RunbookRunPreviewResource GetPreview(DeploymentPromotionTarget promotionTarget);
        RunbookSnapshotResource SnapshotVariables(RunbookSnapshotResource runbookSnapshot);    
        RunbookSnapshotResource Create(RunbookSnapshotResource runbookSnapshot);
    }
    
    class RunbookSnapshotRepository : BasicRepository<RunbookSnapshotResource>, IRunbookSnapshotRepository
    {
        public RunbookSnapshotRepository(IOctopusRepository repository)
            : base(repository, "RunbookSnapshots")
        {
        }

        public ResourceCollection<RunbookRunResource> GetRunbookRuns(RunbookSnapshotResource runbookSnapshot, int skip = 0, int? take = null)
        {
            return Client.List<RunbookRunResource>(runbookSnapshot.Link("RunbookRuns"), new { skip, take });
        }

        public ResourceCollection<ArtifactResource> GetArtifacts(RunbookSnapshotResource runbookSnapshot, int skip = 0, int? take = null)
        {
            return Client.List<ArtifactResource>(runbookSnapshot.Link("Artifacts"), new { skip, take });
        }

        public RunbookRunTemplateResource GetTemplate(RunbookSnapshotResource runbookSnapshot)
        {
            return Client.Get<RunbookRunTemplateResource>(runbookSnapshot.Link("RunbookRunTemplate"));
        }

        public RunbookRunPreviewResource GetPreview(DeploymentPromotionTarget promotionTarget)
        {
            return Client.Get<RunbookRunPreviewResource>(promotionTarget.Link("RunbookRunPreview"));
        }

        public RunbookSnapshotResource SnapshotVariables(RunbookSnapshotResource runbookSnapshot)
        {
            Client.Post(runbookSnapshot.Link("SnapshotVariables"));
            return Get(runbookSnapshot.Id);
        }

        public RunbookSnapshotResource Create(RunbookSnapshotResource runbookSnapshot)
        {
            return Client.Create(Repository.Link(CollectionLinkName), runbookSnapshot);
        }
    }
}