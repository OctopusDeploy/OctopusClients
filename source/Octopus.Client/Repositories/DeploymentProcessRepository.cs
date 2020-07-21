using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IDeploymentProcessRepository : IGet<DeploymentProcessResource>, IModify<DeploymentProcessResource>
    {
        IDeploymentProcessRepositoryBeta Beta(bool useBeta);
        ReleaseTemplateResource GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel);
    }

    class DeploymentProcessRepository : BasicRepository<DeploymentProcessResource>, IDeploymentProcessRepository
    {
        private readonly DeploymentProcessRepositoryBeta beta;

        public DeploymentProcessRepository(IOctopusRepository repository)
            : base(repository, "DeploymentProcesses")
        {
            beta = new DeploymentProcessRepositoryBeta(repository);
        }

        public IDeploymentProcessRepositoryBeta Beta(bool useBeta)
        {
            if (!useBeta) throw new Exception($"You must supply true for {nameof(useBeta)} to use Beta functionality.");

            return beta;
        }

        public ReleaseTemplateResource GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel)
        {
            return Client.Get<ReleaseTemplateResource>(deploymentProcess.Link("Template"), new { channel = channel?.Id });
        }
    }

    public interface IDeploymentProcessRepositoryBeta
    {
        DeploymentProcessResource Get(ProjectResource projectResource, string gitref = null);
    }

    class DeploymentProcessRepositoryBeta : IDeploymentProcessRepositoryBeta
    {
        private readonly IOctopusRepository repository;
        private readonly IOctopusClient client;

        public DeploymentProcessRepositoryBeta(IOctopusRepository repository)
        {
            this.repository = repository;
            this.client = repository.Client;
        }

        public DeploymentProcessResource Get(ProjectResource projectResource, string gitref = null)
        {
            if (!string.IsNullOrWhiteSpace(gitref))
            {
                var branchResource = repository.Projects.Beta(true).GetVersionControlledBranch(projectResource, gitref);

                return client.Get<DeploymentProcessResource>(branchResource.Link("DeploymentProcess"));
            }

            return client.Get<DeploymentProcessResource>(projectResource.Link("DeploymentProcess"));
        }
    }
}