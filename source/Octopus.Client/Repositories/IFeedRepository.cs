using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IFeedRepository : ICreate<FeedResource>, IModify<FeedResource>, IDelete<FeedResource>, IGet<FeedResource>, IFindByName<FeedResource>
    {
        Task<List<PackageResource>> GetVersions(FeedResource feed, string[] packageIds);
    }
}