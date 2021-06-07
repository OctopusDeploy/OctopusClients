using System;
using Octopus.Client.Editors;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IChannelRepository : ICreate<ChannelResource>, ICreateProjectScoped<ChannelResource>, IModify<ChannelResource>, IGet<ChannelResource>, IDelete<ChannelResource>, IPaginate<ChannelResource>, IGetAll<ChannelResource>
    {
        IChannelBetaRepository Beta();
        ChannelResource FindByName(ProjectResource project, string name);
        ChannelEditor CreateOrModify(ProjectResource project, string name);
        ChannelEditor CreateOrModify(ProjectResource project, string name, string description);
    }

    class ChannelRepository : BasicRepository<ChannelResource>, IChannelRepository
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

        public override ChannelResource Create(ChannelResource resource, object pathParameters = null)
        {
            ThrowIfServerVersionIsNotCompatible();

            var projectResource = Repository.Projects.Get(resource.ProjectId);
            if (projectResource.PersistenceSettings.Type == PersistenceSettingsType.VersionControlled)
            {
                // Use the Project-scoped Create method if the parent project is version controlled
                return Create(projectResource, resource, pathParameters);
            }

            // Use the default one otherwise
            return base.Create(resource, pathParameters);
        }

        public ChannelResource Create(ProjectResource projectResource, ChannelResource channelResource, object pathParameters = null)
        {
            ThrowIfServerVersionIsNotCompatible();

            var link = projectResource.Link(CollectionLinkName);
            EnrichSpaceId(channelResource);
            return Client.Create(link, channelResource, pathParameters);
        }
    }

    public interface IChannelBetaRepository
    {
        ChannelResource Create(ProjectResource projectResource, string gitRef, ChannelResource channelResource, object pathParameters = null);
    }

    internal class ChannelBetaRepository : IChannelBetaRepository
    {
        private readonly IOctopusClient client;

        public ChannelBetaRepository(IOctopusRepository repository)
        {
            client = repository.Client;
        }

        public ChannelResource Create(
            ProjectResource projectResource, 
            string gitRef,
            ChannelResource channelResource,
            object pathParameters = null)
        {
            var branch = client.Get<VersionControlBranchResource>(projectResource.Link("Branches"), new { name = gitRef });
            var link = branch.Link("Channels");
            return client.Create(link, channelResource, pathParameters);
        }
    }
}