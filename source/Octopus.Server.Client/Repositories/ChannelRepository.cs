using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IChannelRepository : ICreate<ChannelResource>, ICreateProjectScoped<ChannelResource>, IModify<ChannelResource>, IGet<ChannelResource>, IGetProjectScoped<ChannelResource>, IDelete<ChannelResource>, IPaginate<ChannelResource>, IGetAll<ChannelResource>
    {
        IChannelBetaRepository Beta();
        ChannelResource FindByName(ProjectResource project, string name);
        ChannelEditor CreateOrModify(ProjectResource project, string name);
        ChannelEditor CreateOrModify(ProjectResource project, string name, string description);
    }

    class ChannelRepository : ProjectScopedRepository<ChannelResource>, IChannelRepository
    {
        private readonly IChannelBetaRepository beta;
        public ChannelRepository(IOctopusRepository repository) : base(repository, "Channels")
        {
            beta = new ChannelBetaRepository(repository);
        }

        public IChannelBetaRepository Beta() => beta;

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

    public interface IChannelBetaRepository
    {
        ChannelResource Create(ProjectResource projectResource, ChannelResource channelResource, string gitRef = null, string commitMessage = null);
    }

    internal class ChannelBetaRepository : IChannelBetaRepository
    {
        private readonly IOctopusRepository repository;
        private readonly IOctopusClient client;

        public ChannelBetaRepository(IOctopusRepository repository)
        {
            this.repository = repository;
            client = repository.Client;
        }

        public ChannelResource Get(ProjectResource projectResource, string idOrHref, string gitRef = null)
        {
            if (!(projectResource.PersistenceSettings is VersionControlSettingsResource settings))
                return repository.Channels.Get(projectResource, idOrHref);
            
            gitRef = gitRef ?? settings.DefaultBranch;
            
            var link = projectResource.Link("Channels");
            return client.Get<ChannelResource>(link, new { id = idOrHref, gitRef });
        }

        public ChannelResource Create(
            ProjectResource projectResource, 
            ChannelResource channelResource,
            string gitRef = null,
            string commitMessage = null)
        {
            if (!(projectResource.PersistenceSettings is VersionControlSettingsResource settings))
                return repository.Channels.Create(projectResource, channelResource);
            
            gitRef = gitRef ?? settings.DefaultBranch;
            
            var link = projectResource.Link("Channels");

            var commitResouce = new CommitResource<ChannelResource>
            {
                CommitMessage = commitMessage,
                Resource = channelResource
            };
            
            return client.Post<CommitResource<ChannelResource>, ChannelResource>(link, commitResouce, new { gitRef });
        }
    }
}