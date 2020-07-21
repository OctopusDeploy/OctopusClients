using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IDeploymentProcessRepository : IGet<DeploymentProcessResource>, IModify<DeploymentProcessResource>
    {
        IDeploymentProcessRepositoryBeta Beta(bool useBeta);
        Task<ReleaseTemplateResource> GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel);
    }

    class DeploymentProcessRepository : BasicRepository<DeploymentProcessResource>, IDeploymentProcessRepository
    {
        private readonly IDeploymentProcessRepositoryBeta beta;

        public DeploymentProcessRepository(IOctopusAsyncRepository repository)
            : base(repository, "DeploymentProcesses")
        {
            beta = new DeploymentProcessRepositoryBeta(repository);
        }

        public IDeploymentProcessRepositoryBeta Beta(bool useBeta)
        {
            if (!useBeta) throw new Exception($"You must supply true for {nameof(useBeta)} to use Beta functionality.");

            return beta;
        }

        public Task<ReleaseTemplateResource> GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel)
        {
            return Client.Get<ReleaseTemplateResource>(deploymentProcess.Link("Template"), new { channel = channel?.Id });
        }
    }

    public interface IDeploymentProcessRepositoryBeta
    {
        Task<DeploymentProcessResource> Get(ProjectResource projectResource, string gitref = null);
    }

    class DeploymentProcessRepositoryBeta : IDeploymentProcessRepositoryBeta
    {
        private readonly IOctopusAsyncRepository repository;
        private readonly IOctopusAsyncClient client;

        public DeploymentProcessRepositoryBeta(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
            this.client = repository.Client;
        }

        public async Task<DeploymentProcessResource> Get(ProjectResource projectResource, string gitref = null)
        {
            if (!string.IsNullOrWhiteSpace(gitref))
            {
                var branchResource = await repository.Projects.Beta(true).GetVersionControlledBranch(projectResource, gitref);

                return await client.Get<DeploymentProcessResource>(branchResource.Link("DeploymentProcess"));
            }

            return await client.Get<DeploymentProcessResource>(projectResource.Link("DeploymentProcess"));
        }
    }
}
