using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

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
        Task<ChannelResource> Create(ProjectResource projectResource, string gitRef, ChannelResource channelResource, object pathParameters = null);
    }

    internal class ChannelBetaRepository : IChannelBetaRepository
    {
        private readonly IOctopusAsyncClient client;

        public ChannelBetaRepository(IOctopusAsyncRepository repository)
        {
            client = repository.Client;
        }

        public async Task<ChannelResource> Create(
            ProjectResource projectResource, 
            string gitRef,
            ChannelResource channelResource,
            object pathParameters = null)
        {
            var branch = await client.Get<VersionControlBranchResource>(projectResource.Link("Branches"), new { name = gitRef });
            var link = branch.Link("Channels");
            return await client.Create(link, channelResource, pathParameters);
        }
    }
}
