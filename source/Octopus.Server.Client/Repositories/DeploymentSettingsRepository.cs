using System;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Repositories
{
    public interface IDeploymentSettingsRepository
    {
        /// <summary>
        /// Get Deployment process for Version Controlled Project.
        /// </summary>
        /// <param name="projectResource"></param>
        /// <param name="gitRef">The git branch, tag or commit to source this resource from</param>
        /// <returns></returns>
        DeploymentSettingsResource Get(ProjectResource projectResource, string gitRef);

        DeploymentSettingsResource Get(ProjectResource project);
        
        [Obsolete("ProjectResource is no longer required to be passed in")]
        DeploymentSettingsResource Modify(ProjectResource project, DeploymentSettingsResource deploymentSettings);

        DeploymentSettingsResource Modify(DeploymentSettingsResource deploymentSettings);
        
        /// <summary>
        /// This overload is only relevant for VCS Projects. If passed a database backed deployment setting, the commit message will be ignored.
        /// </summary>
        DeploymentSettingsResource Modify(DeploymentSettingsResource deploymentSettings, string commitMessage);
    }

    class DeploymentSettingsRepository : IDeploymentSettingsRepository
    {
        private readonly IOctopusClient client;

        public DeploymentSettingsRepository(IOctopusRepository repository)
        {
            client = repository.Client;
        }

        public DeploymentSettingsResource Get(ProjectResource projectResource, string gitRef)
        {
            if (!projectResource.IsVersionControlled)
            {
                throw new NotSupportedException(
                    $"Database backed projects require using the overload that does not include a gitRef parameter.");
            }

            return client.Get<DeploymentSettingsResource>(projectResource.Link("DeploymentSettings"), new {gitRef});
        }

        public DeploymentSettingsResource Get(ProjectResource project)
        {
            string gitRef = null;
            if (project.PersistenceSettings is GitPersistenceSettingsResource vcsResource)
            {
                gitRef = vcsResource.DefaultBranch;
            }
            return client.Get<DeploymentSettingsResource>(project.Link("DeploymentSettings"),new {gitRef});
        }
        
        public DeploymentSettingsResource Modify(ProjectResource project,
            DeploymentSettingsResource deploymentSettings)
        {
            return Modify(deploymentSettings);
        }

        public DeploymentSettingsResource Modify(DeploymentSettingsResource deploymentSettings)
        {
            client.Put(deploymentSettings.Link("Self"), deploymentSettings);
            return client.Get<DeploymentSettingsResource>(deploymentSettings.Link("Self"));
        }

        public DeploymentSettingsResource Modify(DeploymentSettingsResource deploymentSettings, string commitMessage)
        {
            // TODO: revisit/obsolete this API when we have converters
            // until then we need a way to re-use the response from previous client calls
            var json = Serializer.Serialize(deploymentSettings);
            var command = Serializer.Deserialize<ModifyDeploymentSettingsCommand>(json);
            
            command.ChangeDescription = commitMessage;
            
            return Modify(command);
        }
    }
}