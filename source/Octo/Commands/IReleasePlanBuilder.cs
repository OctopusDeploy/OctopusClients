using Octopus.Client;
using Octopus.Client.Model;

namespace Octopus.Cli.Commands
{
    public interface IReleasePlanBuilder
    {
        ReleasePlan Build(IOctopusRepository repository, ProjectResource project, ChannelResource channel, string versionPreReleaseTag);
    }
}