using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IDefectsRepository
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<DefectResource>> GetDefects(ReleaseResource release);
        Task<ResourceCollection<DefectResource>> GetDefects(ReleaseResource release, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task RaiseDefect(ReleaseResource release, string description);
        Task RaiseDefect(ReleaseResource release, string description, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task ResolveDefect(ReleaseResource release);
        Task ResolveDefect(ReleaseResource release, CancellationToken cancellationToken);
    }

    class DefectsRepository : BasicRepository<DefectResource>, IDefectsRepository
    {
        public DefectsRepository(IOctopusAsyncRepository repository)
            : base(repository, "Defects")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ResourceCollection<DefectResource>> GetDefects(ReleaseResource release)
        {
            return GetDefects(release, CancellationToken.None);
        }

        public Task<ResourceCollection<DefectResource>> GetDefects(ReleaseResource release, CancellationToken cancellationToken)
        {
            return Client.List<DefectResource>(release.Link("Defects"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task RaiseDefect(ReleaseResource release, string description)
        {
            return RaiseDefect(release, description, CancellationToken.None);
        }

        public Task RaiseDefect(ReleaseResource release, string description, CancellationToken cancellationToken)
        {
            return Client.Post(release.Link("ReportDefect"), new DefectResource(description), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task ResolveDefect(ReleaseResource release)
        {
            return ResolveDefect(release, CancellationToken.None);
        }

        public Task ResolveDefect(ReleaseResource release, CancellationToken cancellationToken)
        {
            return Client.Post(release.Link("ResolveDefect"), cancellationToken);
        }
    }
}
