using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IChannelRepository : ICreate<ChannelResource>, IModify<ChannelResource>, IGet<ChannelResource>, IDelete<ChannelResource>, IPaginate<ChannelResource>
    {
        Task<ChannelResource> FindByName(ProjectResource project, string name);
        Task<ChannelEditor> CreateOrModify(ProjectResource project, string name);
        Task<ChannelEditor> CreateOrModify(ProjectResource project, string name, string description);
        Task<ResourceCollection<ReleaseResource>> GetReleases(ChannelResource channel,
            int skip = 0, int? take = null, string searchByVersion = null);
        Task<IReadOnlyList<ReleaseResource>> GetAllReleases(ChannelResource channel);
        Task<ReleaseResource> GetReleaseByVersion(ChannelResource channel, string version);
    }

    class ChannelRepository : BasicRepository<ChannelResource>, IChannelRepository
    {
        public ChannelRepository(IOctopusAsyncRepository repository)
            : base(repository, "Channels")
        {
        }

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
            return Client.List<ReleaseResource>(channel.GetReleasesLink(), new { skip, take, searchByVersion });
        }

        public Task<IReadOnlyList<ReleaseResource>> GetAllReleases(ChannelResource channel)
        {
            return Client.ListAll<ReleaseResource>(channel.GetReleasesLink());
        }

        public Task<ReleaseResource> GetReleaseByVersion(ChannelResource channel, string version)
        {
            return Client.Get<ReleaseResource>(channel.GetReleasesLink(), new { version });
        }
    }
}
