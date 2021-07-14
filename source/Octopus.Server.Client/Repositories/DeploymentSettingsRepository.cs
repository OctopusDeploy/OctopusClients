using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IDeploymentSettingsRepository
    {
        IDeploymentSettingsBetaRepository Beta();
    }

    class DeploymentSettingsRepository : IDeploymentSettingsRepository
    {
        private readonly IDeploymentSettingsBetaRepository beta;

        public DeploymentSettingsRepository(IOctopusRepository repository)
        {
            beta = new DeploymentSettingsBetaRepository(repository);
        }

        public IDeploymentSettingsBetaRepository Beta()
        {
            return beta;
        }
    }

    public interface IDeploymentSettingsBetaRepository
    {
        DeploymentSettingsResource Get(ProjectResource project, string gitRef = null);
        DeploymentSettingsResource Modify(ProjectResource project, DeploymentSettingsResource resource, string commitMessage = null);
    }

    class DeploymentSettingsBetaRepository : IDeploymentSettingsBetaRepository
    {
        private readonly IOctopusRepository repository;
        private readonly IOctopusClient client;

        public DeploymentSettingsBetaRepository(IOctopusRepository repository)
        {
            this.repository = repository;
            client = repository.Client;
        }

        public DeploymentSettingsResource Get(ProjectResource projectResource, string gitRef = null)
        {
            if (!string.IsNullOrWhiteSpace(gitRef))
            {
                var branchResource = repository.Projects.Beta().GetVersionControlledBranch(projectResource, gitRef);

                return client.Get<DeploymentSettingsResource>(branchResource.Link("DeploymentSettings"));
            }

            return client.Get<DeploymentSettingsResource>(projectResource.Link("DeploymentSettings"));
        }

        public DeploymentSettingsResource Modify(ProjectResource projectResource, DeploymentSettingsResource resource, string commitMessage = null)
        {
            if (!projectResource.IsVersionControlled)
            {
                return client.Update(projectResource.Link("DeploymentSettings"), resource);
            }

            var commitResource = new CommitResource<DeploymentSettingsResource>
            {
                Resource = resource,
                CommitMessage = commitMessage
            };

            client.Put(resource.Link("Self"), commitResource);

            return client.Get<DeploymentSettingsResource>(resource.Link("Self"));
        }
    }
}