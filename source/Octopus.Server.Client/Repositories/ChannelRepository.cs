using System;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IChannelRepository : ICreate<ChannelResource>, IModify<ChannelResource>, IGet<ChannelResource>, IDelete<ChannelResource>, IPaginate<ChannelResource>, IGetAll<ChannelResource>
    {
        ChannelResource FindByName(ProjectResource project, string name);
        ChannelEditor CreateOrModify(ProjectResource project, string name);
        ChannelEditor CreateOrModify(ProjectResource project, string name, string description);
    }
    
    class ChannelRepository : BasicRepository<ChannelResource>, IChannelRepository
    {
        public ChannelRepository(IOctopusRepository repository)
            : base(repository, "Channels")
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

        public override ChannelResource Create(ChannelResource resource, object pathParameters = null)
        {
            var projectResource = Repository.Projects.Get(resource.ProjectId);
            if (projectResource.PersistenceSettings.Type == PersistenceSettingsType.VersionControlled)
            {
                return Create(projectResource, resource, pathParameters);
            }

            return base.Create(resource, pathParameters);
        }

        ChannelResource Create(ProjectResource projectResource, ChannelResource channelResource, object pathParameters = null)
        {
            ThrowIfServerVersionIsNotCompatible();

            var link = projectResource.Links["Channels"];
            EnrichSpaceId(channelResource);
            return Client.Create(link, channelResource, pathParameters);
        }
    }
}