using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Repositories.Async
{
    public interface IDeploymentSettingsRepository
    {
        Task<DeploymentSettingsResource> Get(ProjectResource project);
        Task<DeploymentSettingsResource> Get(ProjectResource projectResource, string gitRef);
        Task<DeploymentSettingsResource> Modify(ProjectResource project, DeploymentSettingsResource deploymentSettings);
    }

    internal class DeploymentSettingsRepository : IDeploymentSettingsRepository
    {
        private readonly IOctopusAsyncClient client;

        public DeploymentSettingsRepository(IOctopusAsyncRepository repository)
        {
            client = repository.Client;
        }

        public async Task<DeploymentSettingsResource> Get(ProjectResource projectResource, string gitRef)
        {
            if (projectResource.IsVersionControlled)
                throw new NotSupportedException(
                    $"Database backed projects require using the overload that does not include a gitRef parameter.");
            
            return await client.Get<DeploymentSettingsResource>(projectResource.Link("DeploymentSettings"), new {gitRef});
        }
        
        public async Task<DeploymentSettingsResource> Modify(ProjectResource projectResource, DeploymentSettingsResource resource, string commitMessage)
        {
            if (!projectResource.IsVersionControlled)
                throw new NotSupportedException($"Database backed projects do not support supplying commit messages.");
            
            // TODO: revisit/obsolete this API when we have converters
            // until then we need a way to re-use the response from previous client calls
            var json = Serializer.Serialize(resource);
            var command = Serializer.Deserialize<ModifyDeploymentSettingsCommand>(json);
            
            command.ChangeDescription = commitMessage;
            
            await client.Update(command.Link("Self"), command);
            return await client.Get<DeploymentSettingsResource>(command.Link("Self"));
        }
        
        public async Task<DeploymentSettingsResource> Get(ProjectResource projectResource)
        {
            if (projectResource.IsVersionControlled)
                throw new NotSupportedException(
                    $"Version Controlled projects require using the overload that includes a gitRef parameter.");

            return await client.Get<DeploymentSettingsResource>(projectResource.Link("DeploymentSettings"));
        }

        public async Task<DeploymentSettingsResource> Modify(ProjectResource projectResource, DeploymentSettingsResource deploymentSettings)
        {
            await client.Put(deploymentSettings.Link("Self"), deploymentSettings);

            return await client.Get<DeploymentSettingsResource>(deploymentSettings.Link("Self"));
        }
    }

}