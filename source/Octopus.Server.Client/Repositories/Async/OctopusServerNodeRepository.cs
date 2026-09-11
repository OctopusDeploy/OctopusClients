using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IOctopusServerNodeRepository : IModify<OctopusServerNodeResource>, IDelete<OctopusServerNodeResource>, IGet<OctopusServerNodeResource>, IFindByName<OctopusServerNodeResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<OctopusServerNodeDetailsResource> Details(OctopusServerNodeResource node);
        Task<OctopusServerNodeDetailsResource> Details(OctopusServerNodeResource node, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<OctopusServerClusterSummaryResource> Summary();
        Task<OctopusServerClusterSummaryResource> Summary(CancellationToken cancellationToken);
    }

    class OctopusServerNodeRepository : BasicRepository<OctopusServerNodeResource>, IOctopusServerNodeRepository
    {
        private readonly IOctopusAsyncRepository repository;

        public OctopusServerNodeRepository(IOctopusAsyncRepository repository)
            : base(repository, "OctopusServerNodes")
        {
            this.repository = repository;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<OctopusServerNodeDetailsResource> Details(OctopusServerNodeResource node)
            => Details(node, CancellationToken.None);

        public async Task<OctopusServerNodeDetailsResource> Details(OctopusServerNodeResource node, CancellationToken cancellationToken)
        {
            return await repository.Client.Get<OctopusServerNodeDetailsResource>(node.Link("Details"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<OctopusServerClusterSummaryResource> Summary()
            => Summary(CancellationToken.None);

        public async Task<OctopusServerClusterSummaryResource> Summary(CancellationToken cancellationToken)
        {
            return await repository.Client.Get<OctopusServerClusterSummaryResource>(await repository.Link("OctopusServerClusterSummary").ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
        }
    }
}
