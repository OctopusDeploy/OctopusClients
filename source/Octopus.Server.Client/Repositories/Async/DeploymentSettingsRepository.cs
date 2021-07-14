using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IDeploymentSettingsRepository
    {
        IDeploymentSettingsBetaRepository Beta();

        Task<DeploymentSettingsResource> Get(ProjectResource project);
        Task<DeploymentSettingsResource> Modify(ProjectResource project, DeploymentSettingsResource deploymentSettings);
    }

    internal class DeploymentSettingsRepository : IDeploymentSettingsRepository
    {
        private readonly IDeploymentSettingsBetaRepository beta;
        private readonly IOctopusAsyncClient client;

        public DeploymentSettingsRepository(IOctopusAsyncRepository repository)
        {
            beta = new DeploymentSettingsBetaRepository(repository);
            client = repository.Client;
        }

        public IDeploymentSettingsBetaRepository Beta()
        {
            return beta;
        }

        public async Task<DeploymentSettingsResource> Get(ProjectResource project)
        {
            if (project.PersistenceSettings is VersionControlSettingsResource)
                throw new NotSupportedException(
                    $"Version Controlled projects are still in Beta. Use {nameof(IDeploymentSettingsBetaRepository)}.");

            return await client.Get<DeploymentSettingsResource>(project.Link("DeploymentSettings"));
        }

        public async Task<DeploymentSettingsResource> Modify(ProjectResource project,
            DeploymentSettingsResource deploymentSettings)
        {
            if (project.PersistenceSettings is VersionControlSettingsResource)
                throw new NotSupportedException(
                    $"Version Controlled projects are still in Beta. Use {nameof(IDeploymentSettingsBetaRepository)}.");

            await client.Put(deploymentSettings.Link("Self"), deploymentSettings);

            return await client.Get<DeploymentSettingsResource>(deploymentSettings.Link("Self"));
        }
    }

    public interface IDeploymentSettingsBetaRepository
    {
        Task<DeploymentSettingsResource> Get(ProjectResource project, string gitRef = null);

        Task<DeploymentSettingsResource> Modify(ProjectResource project, DeploymentSettingsResource resource,
            string commitMessage = null);
    }

    internal class DeploymentSettingsBetaRepository : IDeploymentSettingsBetaRepository
    {
        private readonly IOctopusAsyncClient client;
        private readonly IOctopusAsyncRepository repository;

        public DeploymentSettingsBetaRepository(IOctopusAsyncRepository repository)
        {
            this.repository = repository;
            client = repository.Client;
        }

        public async Task<DeploymentSettingsResource> Get(ProjectResource project, string gitRef = null)
        {
            if (!(project.PersistenceSettings is VersionControlSettingsResource settings))
                return await repository.DeploymentSettings.Get(project);

            gitRef = gitRef ?? settings.DefaultBranch;
            var branch = await repository.Projects.Beta().GetVersionControlledBranch(project, gitRef);

            return await client.Get<DeploymentSettingsResource>(branch.Link("DeploymentSettings"));
        }

        public async Task<DeploymentSettingsResource> Modify(ProjectResource project,
            DeploymentSettingsResource resource, string commitMessage = null)
        {
            if (!(project.PersistenceSettings is VersionControlSettingsResource))
                return await repository.DeploymentSettings.Modify(project, resource);

            var commit = new CommitResource<DeploymentSettingsResource>
            {
                Resource = resource,
                CommitMessage = commitMessage
            };

            await client.Put(resource.Link("Self"), commit);

            return await client.Get<DeploymentSettingsResource>(resource.Link("Self"));
        }
    }
}