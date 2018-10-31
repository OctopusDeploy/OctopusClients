using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IFeedRepository : ICreate<FeedResource>, IModify<FeedResource>, IDelete<FeedResource>, IGet<FeedResource>, IFindByName<FeedResource>
    {
        List<PackageResource> GetVersions(FeedResource feed, string[] packageIds);
    }
    
    class FeedRepository : BasicRepository<FeedResource>, IFeedRepository
    {
        public FeedRepository(IOctopusRepository repository) : base(repository, "Feeds")
        {
        }

        public List<PackageResource> GetVersions(FeedResource feed, string[] packageIds)
        {
            return Client.Get<List<PackageResource>>(feed.Link("VersionsTemplate"), new { packageIds = packageIds });
        }
    }
}