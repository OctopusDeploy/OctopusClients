using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IFeedRepository : ICreate<FeedResource>, IModify<FeedResource>, IDelete<FeedResource>, IGet<FeedResource>, IFindByName<FeedResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<PackageResource>> GetVersions(FeedResource feed, string[] packageIds);
        Task<List<PackageResource>> GetVersions(FeedResource feed, string[] packageIds, CancellationToken cancellationToken);
        
        Task<List<PackageResource>> GetVersions(FeedResource feed, string[] packageIds, bool includeAllVersions, CancellationToken cancellationToken);
    }

    class FeedRepository : BasicRepository<FeedResource>, IFeedRepository
    {
        public FeedRepository(IOctopusAsyncRepository repository) : base(repository, "Feeds")
        {
        }

        public Task<List<PackageResource>> GetVersions(FeedResource feed, string[] packageIds)
        {
            return GetVersions(feed, packageIds, CancellationToken.None);
        }
        
        public Task<List<PackageResource>> GetVersions(FeedResource feed, string[] packageIds, CancellationToken cancellationToken)
        {
            return Client.Get<List<PackageResource>>(feed.Link("VersionsTemplate"), new { packageIds = packageIds }, cancellationToken);
        }
        
        public Task<List<PackageResource>> GetVersions(FeedResource feed, string[] packageIds, bool IncludeMultipleVersions, CancellationToken cancellationToken)
        {
            return Client.Get<List<PackageResource>>(feed.Link("VersionsTemplate"), 
                new { packageIds = packageIds, includeAllVersions = IncludeMultipleVersions },
                cancellationToken);
        }
    }
}
