using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IDefectsRepository
    {
        ResourceCollection<DefectResource> GetDefects(ReleaseResource release);
        void RaiseDefect(ReleaseResource release, string description);
        void ResolveDefect(ReleaseResource release);
    }
    
    class DefectsRepository : BasicRepository<DefectResource>, IDefectsRepository
    {
        public DefectsRepository(IOctopusRepository repository)
            : base(repository, "Defects")
        {
        }

        public ResourceCollection<DefectResource> GetDefects(ReleaseResource release)
        {
            return Client.List<DefectResource>(release.Link("Defects"));
        }

        public void RaiseDefect(ReleaseResource release, string description)
        {
            Client.Post(release.Link("ReportDefect"), new DefectResource(description));
        }

        public void ResolveDefect(ReleaseResource release)
        {
            Client.Post(release.Link("ResolveDefect"));
        }
    }
}