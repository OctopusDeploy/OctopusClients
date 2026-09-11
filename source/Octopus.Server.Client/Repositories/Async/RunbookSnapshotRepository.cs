using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IRunbookSnapshotRepository : IGet<RunbookSnapshotResource>, ICreate<RunbookSnapshotResource>, IPaginate<RunbookSnapshotResource>, IModify<RunbookSnapshotResource>, IDelete<RunbookSnapshotResource>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="runbookSnapshot"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.15)</param>
        /// <returns></returns>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<RunbookRunResource>> GetRunbookRuns(RunbookSnapshotResource runbookSnapshot, int skip = 0, int? take = null);
        Task<ResourceCollection<RunbookRunResource>> GetRunbookRuns(RunbookSnapshotResource runbookSnapshot, CancellationToken cancellationToken, int skip = 0, int? take = null);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="runbookSnapshot"></param>
        /// <param name="skip">Number of records to skip</param>
        /// <param name="take">Number of records to take (First supported in Server 3.14.15)</param>
        /// <returns></returns>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<ArtifactResource>> GetArtifacts(RunbookSnapshotResource runbookSnapshot, int skip = 0, int? take = null);
        Task<ResourceCollection<ArtifactResource>> GetArtifacts(RunbookSnapshotResource runbookSnapshot, CancellationToken cancellationToken, int skip = 0, int? take = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<RunbookRunTemplateResource> GetTemplate(RunbookSnapshotResource runbookSnapshot);
        Task<RunbookRunTemplateResource> GetTemplate(RunbookSnapshotResource runbookSnapshot, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<RunbookRunPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget);
        Task<RunbookRunPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<RunbookSnapshotResource> SnapshotVariables(RunbookSnapshotResource runbookSnapshot);
        Task<RunbookSnapshotResource> SnapshotVariables(RunbookSnapshotResource runbookSnapshot, CancellationToken cancellationToken);
        /// <param name="runbookSnapshot"></param>
        /// <param name="variableSnapshotConcurrencyToken">
        /// The VariableSnapshotConcurrencyToken read from the runbook snapshot. When supplied, the update fails with a
        /// conflict if the snapshot's variable snapshots have changed since. Omit to skip the check.
        /// </param>
        /// <param name="cancellationToken">Request cancellation token</param>
        Task<RunbookSnapshotResource> SnapshotVariables(RunbookSnapshotResource runbookSnapshot, string variableSnapshotConcurrencyToken, CancellationToken cancellationToken);
        Task<RunbookSnapshotResource> SnapshotVariablesByName(RunbookSnapshotResource runbookSnapshot, VariableIdentifier[] variables, CancellationToken cancellationToken);
        /// <param name="runbookSnapshot"></param>
        /// <param name="variables"></param>
        /// <param name="variableSnapshotConcurrencyToken">
        /// The VariableSnapshotConcurrencyToken read from the runbook snapshot. When supplied, the update fails with a
        /// conflict if the snapshot's variable snapshots have changed since. Omit to skip the check.
        /// </param>
        /// <param name="cancellationToken">Request cancellation token</param>
        Task<RunbookSnapshotResource> SnapshotVariablesByName(RunbookSnapshotResource runbookSnapshot, VariableIdentifier[] variables, string variableSnapshotConcurrencyToken, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
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
            return GetRunbookRuns(runbookSnapshot, CancellationToken.None, skip, take);
        }

        public Task<ResourceCollection<RunbookRunResource>> GetRunbookRuns(RunbookSnapshotResource runbookSnapshot, CancellationToken cancellationToken, int skip = 0, int? take = null)
        {
            return Client.List<RunbookRunResource>(runbookSnapshot.Link("RunbookRuns"), new { skip, take }, cancellationToken);
        }

        public Task<ResourceCollection<ArtifactResource>> GetArtifacts(RunbookSnapshotResource runbookSnapshot, int skip = 0, int? take = null)
        {
            return GetArtifacts(runbookSnapshot, CancellationToken.None, skip, take);
        }

        public Task<ResourceCollection<ArtifactResource>> GetArtifacts(RunbookSnapshotResource runbookSnapshot, CancellationToken cancellationToken, int skip = 0, int? take = null)
        {
            return Client.List<ArtifactResource>(runbookSnapshot.Link("Artifacts"), new { skip, take }, cancellationToken);
        }

        public Task<RunbookRunTemplateResource> GetTemplate(RunbookSnapshotResource runbookSnapshot)
        {
            return GetTemplate(runbookSnapshot, CancellationToken.None);
        }

        public Task<RunbookRunTemplateResource> GetTemplate(RunbookSnapshotResource runbookSnapshot, CancellationToken cancellationToken)
        {
            return Client.Get<RunbookRunTemplateResource>(runbookSnapshot.Link("RunbookRunTemplate"), cancellationToken);
        }

        public Task<RunbookRunPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget)
        {
            return GetPreview(promotionTarget, CancellationToken.None);
        }

        public Task<RunbookRunPreviewResource> GetPreview(DeploymentPromotionTarget promotionTarget, CancellationToken cancellationToken)
        {
            return Client.Get<RunbookRunPreviewResource>(promotionTarget.Link("RunbookRunPreview"), cancellationToken);
        }

        public Task<RunbookSnapshotResource> SnapshotVariables(RunbookSnapshotResource runbookSnapshot)
        {
            return SnapshotVariables(runbookSnapshot, CancellationToken.None);
        }

        public async Task<RunbookSnapshotResource> SnapshotVariables(RunbookSnapshotResource runbookSnapshot, CancellationToken cancellationToken)
        {
            await Client.Post(runbookSnapshot.Link("SnapshotVariables"), cancellationToken).ConfigureAwait(false);
            return await Get(runbookSnapshot.Id, cancellationToken).ConfigureAwait(false);
        }

        public async Task<RunbookSnapshotResource> SnapshotVariables(RunbookSnapshotResource runbookSnapshot, string variableSnapshotConcurrencyToken, CancellationToken cancellationToken)
        {
            await Client.Post(runbookSnapshot.Link("SnapshotVariables"), new { VariableSnapshotConcurrencyToken = variableSnapshotConcurrencyToken }, cancellationToken).ConfigureAwait(false);
            return await Get(runbookSnapshot.Id).ConfigureAwait(false);
        }

        public async Task<RunbookSnapshotResource> SnapshotVariablesByName(RunbookSnapshotResource runbookSnapshot, VariableIdentifier[] variables, CancellationToken cancellationToken)
        {
            const string route = "~/api/{spaceId}/runbookSnapshots/{runbookSnapshotId}/snapshot-variables-by-name";
            return await Client.Post<object, RunbookSnapshotResource>(
                route,
                new { Variables = variables },
                new { spaceId = runbookSnapshot.SpaceId, runbookSnapshotId = runbookSnapshot.Id },
                cancellationToken
                ).ConfigureAwait(false);
        }

        public async Task<RunbookSnapshotResource> SnapshotVariablesByName(RunbookSnapshotResource runbookSnapshot, VariableIdentifier[] variables, string variableSnapshotConcurrencyToken, CancellationToken cancellationToken)
        {
            const string route = "~/api/{spaceId}/runbookSnapshots/{runbookSnapshotId}/snapshot-variables-by-name";
            return await Client.Post<object, RunbookSnapshotResource>(
                route,
                new { Variables = variables, VariableSnapshotConcurrencyToken = variableSnapshotConcurrencyToken },
                new { spaceId = runbookSnapshot.SpaceId, runbookSnapshotId = runbookSnapshot.Id },
                cancellationToken
                ).ConfigureAwait(false);
        }

        public Task<RunbookSnapshotResource> Create(RunbookSnapshotResource runbookSnapshot)
        {
            return Create(runbookSnapshot, CancellationToken.None);
        }

        public override async Task<RunbookSnapshotResource> Create(RunbookSnapshotResource runbookSnapshot, CancellationToken cancellationToken)
        {
            return await Client.Create(await Repository.Link(CollectionLinkName).ConfigureAwait(false), runbookSnapshot, cancellationToken).ConfigureAwait(false);
        }
    }
}
