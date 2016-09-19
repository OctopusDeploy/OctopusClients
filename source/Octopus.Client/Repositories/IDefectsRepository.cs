using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IDefectsRepository
    {
        Task<ResourceCollection<DefectResource>> GetDefects(ReleaseResource release);
        Task RaiseDefect(ReleaseResource release, string description);
        Task ResolveDefect(ReleaseResource release);
    }
}