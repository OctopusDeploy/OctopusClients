using System;
using System.Linq;
using System.Collections.Generic;
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

        public async Task<ChannelEditor> CreateOrModify(string name)
        {
            var channelBuilder = await new ChannelEditor(repository).CreateOrModify(owner, name).ConfigureAwait(false);
            trackedChannelBuilders.Add(channelBuilder);
            return channelBuilder;
        }

        public async Task<ChannelEditor> CreateOrModify(string name, string description)
        {
            var channelBuilder = await new ChannelEditor(repository).CreateOrModify(owner, name, description).ConfigureAwait(false);
            trackedChannelBuilders.Add(channelBuilder);
            return channelBuilder;
        }

        public async Task<ProjectChannelsEditor> Delete(string name)
        {
            var channel = await repository.FindByName(owner, name).ConfigureAwait(false);
            if (channel != null)
                await repository.Delete(channel).ConfigureAwait(false);
            return this;
        }

        public async Task<ProjectChannelsEditor> SaveAll()
        {
            await Task.WhenAll(
                trackedChannelBuilders.Select(x => x.Save())
            ).ConfigureAwait(false);
            return this;
        }
    }
}