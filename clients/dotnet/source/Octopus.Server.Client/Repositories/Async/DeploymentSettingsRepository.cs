using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Repositories.Async
{
    public interface IDeploymentSettingsRepository
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<DeploymentSettingsResource> Get(ProjectResource project);
        Task<DeploymentSettingsResource> Get(ProjectResource project, CancellationToken cancellationToken);

        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<DeploymentSettingsResource> Get(ProjectResource projectResource, string gitRef);
        Task<DeploymentSettingsResource> Get(ProjectResource projectResource, string gitRef, CancellationToken cancellationToken);
        
        [Obsolete("ProjectResource is no longer required to be passed in")]
        Task<DeploymentSettingsResource> Modify(ProjectResource project, DeploymentSettingsResource deploymentSettings);
        [Obsolete("ProjectResource is no longer required to be passed in")]
        Task<DeploymentSettingsResource> Modify(ProjectResource project, DeploymentSettingsResource deploymentSettings, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<DeploymentSettingsResource> Modify(DeploymentSettingsResource deploymentSettings);
        Task<DeploymentSettingsResource> Modify(DeploymentSettingsResource deploymentSettings, CancellationToken cancellationToken);

        /// <summary>
        /// This overload is only relevant for VCS Projects. If passed a database backed deployment setting, the commit message will be ignored.
        /// </summary>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<DeploymentSettingsResource> Modify(DeploymentSettingsResource resource, string commitMessage);
        Task<DeploymentSettingsResource> Modify(DeploymentSettingsResource resource, string commitMessage, CancellationToken cancellationToken);
    }

    internal class DeploymentSettingsRepository : IDeploymentSettingsRepository
    {
        private readonly IOctopusAsyncClient client;

        public DeploymentSettingsRepository(IOctopusAsyncRepository repository)
        {
            client = repository.Client;
        }

        public Task<DeploymentSettingsResource> Get(ProjectResource projectResource, string gitRef)
            => Get(projectResource, gitRef, CancellationToken.None);

        public async Task<DeploymentSettingsResource> Get(ProjectResource projectResource, string gitRef, CancellationToken cancellationToken)
        {
            if (!projectResource.IsVersionControlled)
            {
                throw new NotSupportedException(
                    $"Database backed projects require using the overload that does not include a gitRef parameter.");
            }

            return await client.Get<DeploymentSettingsResource>(projectResource.Link("DeploymentSettings"), new {gitRef}, cancellationToken);
        }

        public Task<DeploymentSettingsResource> Modify(DeploymentSettingsResource resource, string commitMessage)
            => Modify(resource, commitMessage, CancellationToken.None);

        public async Task<DeploymentSettingsResource> Modify(DeploymentSettingsResource resource, string commitMessage, CancellationToken cancellationToken)
        {
            // TODO: revisit/obsolete this API when we have converters
            // until then we need a way to re-use the response from previous client calls
            var json = Serializer.Serialize(resource);
            var command = Serializer.Deserialize<ModifyDeploymentSettingsCommand>(json);
            
            command.ChangeDescription = commitMessage;
            
            await client.Update(command.Link("Self"), command, cancellationToken);
            return await client.Get<DeploymentSettingsResource>(command.Link("Self"), cancellationToken);
        }

        public Task<DeploymentSettingsResource> Get(ProjectResource projectResource)
            => Get(projectResource, CancellationToken.None);

        public async Task<DeploymentSettingsResource> Get(ProjectResource projectResource, CancellationToken cancellationToken)
        {
            if (projectResource.PersistenceSettings is GitPersistenceSettingsResource vcsResource)
            {
                return await Get(projectResource, vcsResource.DefaultBranch, cancellationToken);
            }

            return await client.Get<DeploymentSettingsResource>(projectResource.Link("DeploymentSettings"), cancellationToken);
        }

        public Task<DeploymentSettingsResource> Modify(ProjectResource projectResource, DeploymentSettingsResource deploymentSettings)
            => Modify(projectResource, deploymentSettings, CancellationToken.None);

        public Task<DeploymentSettingsResource> Modify(ProjectResource projectResource, DeploymentSettingsResource deploymentSettings, CancellationToken cancellationToken)
            => Modify(deploymentSettings, cancellationToken);

        public Task<DeploymentSettingsResource> Modify(DeploymentSettingsResource deploymentSettings)
            => Modify(deploymentSettings, CancellationToken.None);
        
        public async Task<DeploymentSettingsResource> Modify(DeploymentSettingsResource deploymentSettings, CancellationToken cancellationToken)
        {
            await client.Put(deploymentSettings.Link("Self"), deploymentSettings, cancellationToken);

            return await client.Get<DeploymentSettingsResource>(deploymentSettings.Link("Self"), cancellationToken);
        }
    }
}
