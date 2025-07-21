using System;
using System.Collections.Generic;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
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

        public ChannelEditor CreateOrModify(string name)
        {
            var channelBuilder = new ChannelEditor(repository).CreateOrModify(owner, name);
            trackedChannelBuilders.Add(channelBuilder);
            return channelBuilder;
        }

        public ChannelEditor CreateOrModify(string name, string description)
        {
            var channelBuilder = new ChannelEditor(repository).CreateOrModify(owner, name, description);
            trackedChannelBuilders.Add(channelBuilder);
            return channelBuilder;
        }

        public ProjectChannelsEditor Delete(string name)
        {
            var channel = repository.FindByName(owner, name);
            if (channel != null) repository.Delete(channel);
            trackedChannelBuilders.RemoveAll(x => x.Instance.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return this;
        }

        public ProjectChannelsEditor SaveAll()
        {
            trackedChannelBuilders.ForEach(x => x.Save());
            return this;
        }
    }
}
