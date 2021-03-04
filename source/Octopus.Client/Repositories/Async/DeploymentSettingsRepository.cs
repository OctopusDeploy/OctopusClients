using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IDeploymentSettingsRepository
    {
        IDeploymentSettingsBetaRepository Beta();

        Task<DeploymentSettingsResource> Get(ProjectResource project);
        Task<DeploymentSettingsResource> Modify(ProjectResource project, DeploymentSettingsResource resource);
    }

    class DeploymentSettingsRepository : IDeploymentSettingsRepository
    {
        private readonly IDeploymentSettingsBetaRepository beta;

        public DeploymentSettingsRepository(IOctopusAsyncRepository repository)
        {
            beta = new DeploymentSettingsBetaRepository(repository);
        }
        public IDeploymentSettingsBetaRepository Beta()
        {
            return beta;
        }

        public Task<DeploymentSettingsResource> Get(ProjectResource project)
        {
            return beta.Get(project);
        }

        public Task<DeploymentSettingsResource> Modify(ProjectResource project, DeploymentSettingsResource resource)
        {
            return beta.Modify(project, resource);
        }
    }

    public interface IDeploymentSettingsBetaRepository
    {
        Task<DeploymentSettingsResource> Get(ProjectResource project, string gitref = null);
        Task<DeploymentSettingsResource> Modify(ProjectResource project, DeploymentSettingsResource resource,
            string commitMessage = null);
    }

    class DeploymentSettingsBetaRepository : IDeploymentSettingsBetaRepository
    {
        private readonly IOctopusAsyncRepository repository;
        private readonly IOctopusAsyncClient client;

        public DeploymentSettingsBetaRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
            client = repository.Client;
        }

        public async Task<DeploymentSettingsResource> Get(ProjectResource projectResource, string gitref = null)
        {
            if (!string.IsNullOrWhiteSpace(gitref))
            {
                var branchResource = await repository.Projects.Beta().GetVersionControlledBranch(projectResource, gitref);

                return await client.Get<DeploymentSettingsResource>(branchResource.Link("DeploymentSettings"));
            }

            return await client.Get<DeploymentSettingsResource>(projectResource.Link("DeploymentSettings"));
        }

        public async Task<DeploymentSettingsResource> Modify(ProjectResource projectResource,
            DeploymentSettingsResource resource, string commitMessage = null)
        {
            if (!projectResource.IsVersionControlled)
            {
                return await client.Update(projectResource.Link("DeploymentSettings"), resource);
            }

            var commitResource = new CommitResource<DeploymentSettingsResource>
            {
                Resource = resource,
                CommitMessage = commitMessage
            };

            await client.Put(resource.Link("Self"), commitResource);

            return await client.Get<DeploymentSettingsResource>(resource.Link("Self"));
        }
    }
}