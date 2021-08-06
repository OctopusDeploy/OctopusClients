using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Repositories.Async
{
    public interface IChannelRepository : ICreate<ChannelResource>, ICreateProjectScoped<ChannelResource>, IModify<ChannelResource>, IGet<ChannelResource>, IGetProjectScoped<ChannelResource>, IDelete<ChannelResource>, IPaginate<ChannelResource>
    {
        IChannelBetaRepository Beta();
        Task<ChannelResource> FindByName(ProjectResource project, string name);
        Task<ChannelEditor> CreateOrModify(ProjectResource project, string name);
        Task<ChannelEditor> CreateOrModify(ProjectResource project, string name, string description);
        Task<ResourceCollection<ReleaseResource>> GetReleases(ChannelResource channel,
            int skip = 0, int? take = null, string searchByVersion = null);
        Task<IReadOnlyList<ReleaseResource>> GetAllReleases(ChannelResource channel);
        Task<ReleaseResource> GetReleaseByVersion(ChannelResource channel, string version);
    }

    class ChannelRepository : ProjectScopedRepository<ChannelResource>, IChannelRepository
    {
        private readonly IChannelBetaRepository beta;
        public ChannelRepository(IOctopusAsyncRepository repository) : base(repository, "Channels")
        {
            beta = new ChannelBetaRepository(repository);
        }

        public IChannelBetaRepository Beta() => beta;

        public Task<ChannelResource> FindByName(ProjectResource project, string name)
        {
            return FindByName(name, path: project.Link("Channels"));
        }

        public Task<ChannelEditor> CreateOrModify(ProjectResource project, string name)
        {
            return new ChannelEditor(this).CreateOrModify(project, name);
        }

        public Task<ChannelEditor> CreateOrModify(ProjectResource project, string name, string description)
        {
            return new ChannelEditor(this).CreateOrModify(project, name, description);
        }

        public Task<ResourceCollection<ReleaseResource>> GetReleases(ChannelResource channel,
            int skip = 0, int? take = null, string searchByVersion = null)
        {
            return Client.List<ReleaseResource>(channel.Link("Releases"), new { skip, take, searchByVersion });
        }

        public Task<IReadOnlyList<ReleaseResource>> GetAllReleases(ChannelResource channel)
        {
            return Client.ListAll<ReleaseResource>(channel.Link("Releases"));
        }

        public Task<ReleaseResource> GetReleaseByVersion(ChannelResource channel, string version)
        {
            return Client.Get<ReleaseResource>(channel.Link("Releases"), new { version });
        }
    }

    public interface IChannelBetaRepository
    {
        Task<ChannelResource> Get(ProjectResource projectResource, string idOrHref, string gitRef = null);
        Task<ChannelResource> Create(ProjectResource projectResource, ChannelResource channelResource, string gitRef = null, string commitMessage = null);
        Task<ChannelResource> Create(ProjectResource projectResource, CreateChannelCommand command, string gitRef = null);
        Task<ChannelResource> Modify(ProjectResource projectResource, ChannelResource channelResource, string gitRef = null, string commitMessage = null);
        Task<ChannelResource> Modify(ProjectResource projectResource, ModifyChannelCommand command, string gitRef = null);
    }

    internal class ChannelBetaRepository : IChannelBetaRepository
    {
        private readonly IOctopusAsyncClient client;
        private readonly IOctopusAsyncRepository repository;

        public ChannelBetaRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
            client = repository.Client;
        }

        public async Task<ChannelResource> Get(ProjectResource projectResource, string idOrHref, string gitRef = null)
        {
            if (!(projectResource.PersistenceSettings is VersionControlSettingsResource settings))
                return await repository.Channels.Get(projectResource, idOrHref);
            
            gitRef = gitRef ?? settings.DefaultBranch;
            
            var link = projectResource.Link("Channels");
            return await client.Get<ChannelResource>(link, new { id = idOrHref, gitRef });
        }

        public async Task<ChannelResource> Create(
            ProjectResource projectResource, 
            ChannelResource channelResource,
            string gitRef = null,
            string commitMessage = null)
        {
            // TODO: revisit/obsolete this API when we have converters
            // until then we need a way to re-use the response from previous client calls
            var json = Serializer.Serialize(channelResource);
            var command = Serializer.Deserialize<CreateChannelCommand>(json);
            
            command.ChangeDescription = commitMessage;
            
            return await Create(projectResource, command, gitRef);
        }

        public async Task<ChannelResource> Create(ProjectResource projectResource, CreateChannelCommand command, string gitRef = null)
        {
            if (!(projectResource.PersistenceSettings is VersionControlSettingsResource settings))
                return await repository.Channels.Create(projectResource, command);
            
            gitRef = gitRef ?? settings.DefaultBranch;
            
            var link = projectResource.Link("Channels");
            return await client.Create(link, command, new { gitRef });
        }

        public async Task<ChannelResource> Modify(ProjectResource projectResource, ChannelResource channelResource, string gitRef = null,
            string commitMessage = null)
        {
            // TODO: revisit/obsolete this API when we have converters
            // until then we need a way to re-use the response from previous client calls
            var json = Serializer.Serialize(channelResource);
            var command = Serializer.Deserialize<ModifyChannelCommand>(json);
            
            command.ChangeDescription = commitMessage;
            
            return await Modify(projectResource, command, gitRef);
        }

        public async Task<ChannelResource> Modify(ProjectResource projectResource, ModifyChannelCommand command, string gitRef = null)
        {
            if (!(projectResource.PersistenceSettings is VersionControlSettingsResource settings))
                return await repository.Channels.Modify(command);
            
            gitRef = gitRef ?? settings.DefaultBranch;
            
            var link = command.Link("Self");
            return await client.Update(link, command, new { gitRef });
        }
    }
}