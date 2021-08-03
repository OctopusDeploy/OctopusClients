using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

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

        Task<DeploymentSettingsResource> Modify(ProjectResource project, DeploymentSettingsResource resource, string commitMessage = null);
        Task<DeploymentSettingsResource> Modify(ProjectResource project, ModifyDeploymentSettingsCommand command);
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

            return await client.Get<DeploymentSettingsResource>(project.Link("DeploymentSettings"), new { gitRef });
        }

        public async Task<DeploymentSettingsResource> Modify(ProjectResource projectResource, DeploymentSettingsResource resource, string commitMessage = null)
        {
            // TODO: revisit/obsolete this API when we have converters
            // until then we need a way to re-use the response from previous client calls
            var json = Serializer.Serialize(resource);
            var command = Serializer.Deserialize<ModifyDeploymentSettingsCommand>(json);
            
            command.ChangeDescription = commitMessage;
            
            return await Modify(projectResource, command);
        }

        public async Task<DeploymentSettingsResource> Modify(ProjectResource projectResource, ModifyDeploymentSettingsCommand command)
        {
            if (!projectResource.IsVersionControlled)
            {
                return await client.Update(projectResource.Link("DeploymentSettings"), command);
            }
            
            await client.Update(command.Link("Self"), command);
            return await client.Get<DeploymentSettingsResource>(command.Link("Self"));
        }
    }
}