using System;
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