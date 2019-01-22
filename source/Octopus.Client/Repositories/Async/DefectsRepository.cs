using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IDefectsRepository
    {
        Task<ResourceCollection<DefectResource>> GetDefects(ReleaseResource release);
        Task RaiseDefect(ReleaseResource release, string description);
        Task ResolveDefect(ReleaseResource release);
    }

    class DefectsRepository : BasicRepository<DefectResource>, IDefectsRepository
    {
        public DefectsRepository(IOctopusAsyncRepository repository)
            : base(repository, "Defects")
        {
        }

        public Task<ResourceCollection<DefectResource>> GetDefects(ReleaseResource release)
        {
            return Client.List<DefectResource>(release.Link("Defects"));
        }

        public Task RaiseDefect(ReleaseResource release, string description)
        {
            return Client.Post(release.Link("ReportDefect"), new DefectResource(description));
        }

        public Task ResolveDefect(ReleaseResource release)
        {
            return Client.Post(release.Link("ResolveDefect"));
        }
    }
}
