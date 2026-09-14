using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class ProjectChannelsEditor
    {
        private readonly IChannelRepository repository;
        private readonly ProjectResource owner;
        private readonly List<ChannelEditor> trackedChannelBuilders = new List<ChannelEditor>();

        public ProjectChannelsEditor(IChannelRepository repository, ProjectResource owner)
        {
            this.repository = repository;
            this.owner = owner;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ChannelEditor> CreateOrModify(string name)
            => CreateOrModify(name, CancellationToken.None);

        public async Task<ChannelEditor> CreateOrModify(string name, CancellationToken cancellationToken)
        {
            var channelBuilder = await new ChannelEditor(repository).CreateOrModify(owner, name, cancellationToken).ConfigureAwait(false);
            trackedChannelBuilders.Add(channelBuilder);
            return channelBuilder;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ChannelEditor> CreateOrModify(string name, string description)
            => CreateOrModify(name, description, CancellationToken.None);

        public async Task<ChannelEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken)
        {
            var channelBuilder = await new ChannelEditor(repository).CreateOrModify(owner, name, description, cancellationToken).ConfigureAwait(false);
            trackedChannelBuilders.Add(channelBuilder);
            return channelBuilder;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ProjectChannelsEditor> Delete(string name)
            => Delete(name, CancellationToken.None);

        public async Task<ProjectChannelsEditor> Delete(string name, CancellationToken cancellationToken)
        {
            var channel = await repository.FindByName(owner, name, cancellationToken).ConfigureAwait(false);
            if (channel != null)
                await repository.Delete(channel, cancellationToken).ConfigureAwait(false);
            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ProjectChannelsEditor> SaveAll()
            => SaveAll(CancellationToken.None);

        public async Task<ProjectChannelsEditor> SaveAll(CancellationToken cancellationToken)
        {
            await Task.WhenAll(
                trackedChannelBuilders.Select(x => x.Save(cancellationToken))
            ).ConfigureAwait(false);
            return this;
        }
    }
}
