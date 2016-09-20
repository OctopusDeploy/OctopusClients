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
}