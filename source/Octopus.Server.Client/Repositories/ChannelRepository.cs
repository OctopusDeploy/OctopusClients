using Octopus.Client.Editors;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Repositories
{
    public interface IChannelRepository : ICreate<ChannelResource>, ICreateProjectScoped<ChannelResource>, IModify<ChannelResource>, IGet<ChannelResource>, IGetProjectScoped<ChannelResource>, IDelete<ChannelResource>, IPaginate<ChannelResource>, IGetAll<ChannelResource>
    {
        ChannelResource FindByName(ProjectResource project, string name);
        ChannelEditor CreateOrModify(ProjectResource project, string name);
        ChannelEditor CreateOrModify(ProjectResource project, string name, string description);
    }

    class ChannelRepository : ProjectScopedRepository<ChannelResource>, IChannelRepository
    {
        public ChannelRepository(IOctopusRepository repository) : base(repository, "Channels")
        {
        }

        public ChannelResource FindByName(ProjectResource project, string name)
        {
            return FindByName(name, path: project.Link("Channels"));
        }

        public ChannelEditor CreateOrModify(ProjectResource project, string name)
        {
            return new ChannelEditor(this).CreateOrModify(project, name);
        }

        public ChannelEditor CreateOrModify(ProjectResource project, string name, string description)
        {
            return new ChannelEditor(this).CreateOrModify(project, name, description);
        }
    }
}