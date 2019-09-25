using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IOpsSnapshotRepository : IGet<OpsSnapshotResource>, ICreate<OpsSnapshotResource>, IPaginate<OpsSnapshotResource>, IModify<OpsSnapshotResource>, IDelete<OpsSnapshotResource>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="opsSnapshot"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.159)</param>
        /// <returns></returns>
        ResourceCollection<OpsRunResource> GetOpsRuns(OpsSnapshotResource opsSnapshot, int skip = 0, int? take = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="opsSnapshot"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.159)</param>
        /// <returns></returns>
        ResourceCollection<ArtifactResource> GetArtifacts(OpsSnapshotResource opsSnapshot, int skip = 0, int? take = null);
        OpsRunTemplateResource GetTemplate(OpsSnapshotResource opsSnapshot);
        OpsRunPreviewResource GetPreview(DeploymentPromotionTarget promotionTarget);
        OpsSnapshotResource SnapshotVariables(OpsSnapshotResource opsSnapshot);    
        OpsSnapshotResource Create(OpsSnapshotResource opsSnapshot);
    }
    
    class OpsSnapshotRepository : BasicRepository<OpsSnapshotResource>, IOpsSnapshotRepository
    {
        public OpsSnapshotRepository(IOctopusRepository repository)
            : base(repository, "OpsSnapshots")
        {
        }

        public ResourceCollection<OpsRunResource> GetOpsRuns(OpsSnapshotResource opsSnapshot, int skip = 0, int? take = null)
        {
            return Client.List<OpsRunResource>(opsSnapshot.Link("OpsRuns"), new { skip, take });
        }

        public ResourceCollection<ArtifactResource> GetArtifacts(OpsSnapshotResource opsSnapshot, int skip = 0, int? take = null)
        {
            return Client.List<ArtifactResource>(opsSnapshot.Link("Artifacts"), new { skip, take });
        }

        public OpsRunTemplateResource GetTemplate(OpsSnapshotResource opsSnapshot)
        {
            return Client.Get<OpsRunTemplateResource>(opsSnapshot.Link("OpsRunTemplate"));
        }

        public OpsRunPreviewResource GetPreview(DeploymentPromotionTarget promotionTarget)
        {
            return Client.Get<OpsRunPreviewResource>(promotionTarget.Link("OpsRunPreview"));
        }

        public OpsSnapshotResource SnapshotVariables(OpsSnapshotResource opsSnapshot)
        {
            Client.Post(opsSnapshot.Link("SnapshotVariables"));
            return Get(opsSnapshot.Id);
        }

        public OpsSnapshotResource Create(OpsSnapshotResource opsSnapshot)
        {
            return Client.Create(Repository.Link(CollectionLinkName), opsSnapshot);
        }
    }
}