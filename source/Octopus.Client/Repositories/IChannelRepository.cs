using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IChannelRepository : ICreate<ChannelResource>, IModify<ChannelResource>, IGet<ChannelResource>, IDelete<ChannelResource>, IPaginate<ChannelResource>
    {
        ChannelResource FindByName(ProjectResource project, string name);
        ChannelEditor CreateOrModify(ProjectResource project, string name);
        ChannelEditor CreateOrModify(ProjectResource project, string name, string description);
    }
}