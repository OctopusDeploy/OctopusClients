using System.Threading.Tasks;
using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands.Releases
{
    public interface IReleasePlanBuilder
    {
        Task<ReleasePlan> Build(IOctopusAsyncRepository repository, ProjectResource project, ChannelResource channel, string versionPreReleaseTag);
    }
}