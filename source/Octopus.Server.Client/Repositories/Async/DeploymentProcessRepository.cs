using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Repositories.Async
{
    public interface IDeploymentProcessRepository : IGet<DeploymentProcessResource>, IModify<DeploymentProcessResource>
    {
        IDeploymentProcessBetaRepository Beta();
        Task<ReleaseTemplateResource> GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel);
    }

    class DeploymentProcessRepository : BasicRepository<DeploymentProcessResource>, IDeploymentProcessRepository
    {
        private readonly IDeploymentProcessBetaRepository beta;

        public DeploymentProcessRepository(IOctopusAsyncRepository repository)
            : base(repository, "DeploymentProcesses")
        {
            beta = new DeploymentProcessBetaRepository(repository);
        }

        public IDeploymentProcessBetaRepository Beta()
        {
            return beta;
        }

        public Task<ReleaseTemplateResource> GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel)
        {
            return Client.Get<ReleaseTemplateResource>(deploymentProcess.Link("Template"), new { channel = channel?.Id });
        }
    }

    public interface IDeploymentProcessBetaRepository
    {
        Task<DeploymentProcessResource> Get(ProjectResource projectResource, string gitRef = null);
        Task<DeploymentProcessResource> Modify(ProjectResource projectResource, DeploymentProcessResource resource, string commitMessage = null);
        Task<DeploymentProcessResource> Modify(ProjectResource projectResource, ModifyDeploymentProcessCommand resource);
    }

    class DeploymentProcessBetaRepository : IDeploymentProcessBetaRepository
    {
        private readonly IOctopusAsyncRepository repository;
        private readonly IOctopusAsyncClient client;

        public DeploymentProcessBetaRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
            this.client = repository.Client;
        }

        public async Task<DeploymentProcessResource> Get(ProjectResource projectResource, string gitRef = null)
        {
            if (!(projectResource.PersistenceSettings is VersionControlSettingsResource settings))
                return await repository.DeploymentProcesses.Get(projectResource.DeploymentProcessId);

            gitRef = gitRef ?? settings.DefaultBranch;

            return await client.Get<DeploymentProcessResource>(projectResource.Link("DeploymentProcess"), new { gitRef });
        }

        public async Task<DeploymentProcessResource> Modify(ProjectResource projectResource, DeploymentProcessResource resource, string commitMessage = null)
        {
            // TODO: revisit/obsolete this API when we have converters
            // until then we need a way to re-use the response from previous client calls
            var json = Serializer.Serialize(resource);
            var command = Serializer.Deserialize<ModifyDeploymentProcessCommand>(json);
            
            command.ChangeDescription = commitMessage;
            
            return await Modify(projectResource, command);
        }

        public async Task<DeploymentProcessResource> Modify(ProjectResource projectResource, ModifyDeploymentProcessCommand command)
        {
            if (!projectResource.IsVersionControlled)
            {
                return await client.Update(projectResource.Link("DeploymentProcess"), command);
            }
            
            await client.Update(command.Link("Self"), command);
            return await client.Get<DeploymentProcessResource>(command.Link("Self"));
        }
    }
}
