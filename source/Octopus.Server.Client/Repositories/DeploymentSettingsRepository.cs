using System;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Repositories
{
    public interface IDeploymentSettingsRepository
    {
        IDeploymentSettingsBetaRepository Beta();

        DeploymentSettingsResource Get(ProjectResource project);
        DeploymentSettingsResource Modify(ProjectResource project, DeploymentSettingsResource deploymentSettings);
    }

    class DeploymentSettingsRepository : IDeploymentSettingsRepository
    {
        private readonly IDeploymentSettingsBetaRepository beta;
        private readonly IOctopusClient client;

        public DeploymentSettingsRepository(IOctopusRepository repository)
        {
            beta = new DeploymentSettingsBetaRepository(repository);
            client = repository.Client;
        }

        public IDeploymentSettingsBetaRepository Beta()
        {
            return beta;
        }

        public DeploymentSettingsResource Get(ProjectResource project)
        {
            if (project.PersistenceSettings is VersionControlSettingsResource)
                throw new NotSupportedException(
                    $"Version Controlled projects are still in Beta. Use {nameof(IDeploymentSettingsBetaRepository)}.");

            return client.Get<DeploymentSettingsResource>(project.Link("DeploymentSettings"));
        }

        public DeploymentSettingsResource Modify(ProjectResource project,
            DeploymentSettingsResource deploymentSettings)
        {
            if (project.PersistenceSettings is VersionControlSettingsResource)
                throw new NotSupportedException(
                    $"Version Controlled projects are still in Beta. Use {nameof(IDeploymentSettingsBetaRepository)}.");

            client.Put(deploymentSettings.Link("Self"), deploymentSettings);

            return client.Get<DeploymentSettingsResource>(deploymentSettings.Link("Self"));
        }
    }

    public interface IDeploymentSettingsBetaRepository
    {
        DeploymentSettingsResource Get(ProjectResource project, string gitRef = null);
        DeploymentSettingsResource Modify(ProjectResource project, DeploymentSettingsResource resource, string commitMessage = null);
        DeploymentSettingsResource Modify(ProjectResource projectResource, ModifyDeploymentSettingsCommand command);
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
            if (!(projectResource.PersistenceSettings is VersionControlSettingsResource settings))
                return repository.DeploymentSettings.Get(projectResource);

            gitRef = gitRef ?? settings.DefaultBranch;

            return client.Get<DeploymentSettingsResource>(projectResource.Link("DeploymentSettings"), new { gitRef });
        }

        public DeploymentSettingsResource Modify(ProjectResource projectResource, DeploymentSettingsResource resource, string commitMessage = null)
        {
            // TODO: revisit/obsolete this API when we have converters
            // until then we need a way to re-use the response from previous client calls
            var json = Serializer.Serialize(resource);
            var command = Serializer.Deserialize<ModifyDeploymentSettingsCommand>(json);
            
            command.ChangeDescription = commitMessage;
            
            return Modify(projectResource, command);
        }

        public DeploymentSettingsResource Modify(ProjectResource projectResource, ModifyDeploymentSettingsCommand command)
        {
            if (!(projectResource.PersistenceSettings is VersionControlSettingsResource))
                return repository.DeploymentSettings.Modify(projectResource, command);

            client.Put(command.Link("Self"), command);
            return client.Get<DeploymentSettingsResource>(command.Link("Self"));
        }
    }
}