using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Editors.Async;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IChannelRepository : ICreate<ChannelResource>, IModify<ChannelResource>, IGet<ChannelResource>, IDelete<ChannelResource>, IPaginate<ChannelResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ChannelResource> FindByName(ProjectResource project, string name);
        Task<ChannelResource> FindByName(ProjectResource project, string name, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ChannelEditor> CreateOrModify(ProjectResource project, string name);
        Task<ChannelEditor> CreateOrModify(ProjectResource project, string name, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ChannelEditor> CreateOrModify(ProjectResource project, string name, string description);
        Task<ChannelEditor> CreateOrModify(ProjectResource project, string name, string description, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ResourceCollection<ReleaseResource>> GetReleases(ChannelResource channel,
            int skip = 0, int? take = null, string searchByVersion = null);
        Task<ResourceCollection<ReleaseResource>> GetReleases(ChannelResource channel, CancellationToken cancellationToken,
            int skip = 0, int? take = null, string searchByVersion = null);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<IReadOnlyList<ReleaseResource>> GetAllReleases(ChannelResource channel);
        Task<IReadOnlyList<ReleaseResource>> GetAllReleases(ChannelResource channel, CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ReleaseResource> GetReleaseByVersion(ChannelResource channel, string version);
        Task<ReleaseResource> GetReleaseByVersion(ChannelResource channel, string version, CancellationToken cancellationToken);
    }

    class ChannelRepository : BasicRepository<ChannelResource>, IChannelRepository
    {
        public ChannelRepository(IOctopusAsyncRepository repository)
            : base(repository, "Channels")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ChannelResource> FindByName(ProjectResource project, string name)
            => FindByName(project, name, CancellationToken.None);

        public Task<ChannelResource> FindByName(ProjectResource project, string name, CancellationToken cancellationToken)
        {
            return FindByName(name, project.Link("Channels"), null, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ChannelEditor> CreateOrModify(ProjectResource project, string name)
            => CreateOrModify(project, name, CancellationToken.None);

        public Task<ChannelEditor> CreateOrModify(ProjectResource project, string name, CancellationToken cancellationToken)
        {
            return new ChannelEditor(this).CreateOrModify(project, name);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ChannelEditor> CreateOrModify(ProjectResource project, string name, string description)
            => CreateOrModify(project, name, description, CancellationToken.None);

        public Task<ChannelEditor> CreateOrModify(ProjectResource project, string name, string description, CancellationToken cancellationToken)
        {
            return new ChannelEditor(this).CreateOrModify(project, name, description);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ResourceCollection<ReleaseResource>> GetReleases(ChannelResource channel,
            int skip = 0, int? take = null, string searchByVersion = null)
            => GetReleases(channel, CancellationToken.None, skip, take, searchByVersion);

        public Task<ResourceCollection<ReleaseResource>> GetReleases(ChannelResource channel, CancellationToken cancellationToken,
            int skip = 0, int? take = null, string searchByVersion = null)
        {
            return Client.List<ReleaseResource>(channel.Link("Releases"), new { skip, take, searchByVersion }, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<IReadOnlyList<ReleaseResource>> GetAllReleases(ChannelResource channel)
            => GetAllReleases(channel, CancellationToken.None);

        public Task<IReadOnlyList<ReleaseResource>> GetAllReleases(ChannelResource channel, CancellationToken cancellationToken)
        {
            return Client.ListAll<ReleaseResource>(channel.Link("Releases"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ReleaseResource> GetReleaseByVersion(ChannelResource channel, string version)
            => GetReleaseByVersion(channel, version, CancellationToken.None);

        public Task<ReleaseResource> GetReleaseByVersion(ChannelResource channel, string version, CancellationToken cancellationToken)
        {
            return Client.Get<ReleaseResource>(channel.Link("Releases"), new { version }, cancellationToken);
        }
    }
}
