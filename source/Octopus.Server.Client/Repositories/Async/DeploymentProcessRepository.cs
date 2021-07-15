using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

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

        Task<DeploymentProcessResource> Modify(ProjectResource projectResource, DeploymentProcessResource resource,
            string commitMessage = null);
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

        public async Task<DeploymentProcessResource> Modify(ProjectResource projectResource,
            DeploymentProcessResource resource, string commitMessage = null)
        {
            if (!projectResource.IsVersionControlled)
            {
                return await client.Update(projectResource.Link("DeploymentProcess"), resource);
            }

            var commitResource = new CommitResource<DeploymentProcessResource>
            {
                Resource = resource,
                CommitMessage = commitMessage
            };
            await client.Update(resource.Link("Self"), commitResource);
            return await client.Get<DeploymentProcessResource>(resource.Link("Self"));
        }
    }
}
