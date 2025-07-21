using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Repositories.Async
{
    public interface IDeploymentProcessRepository : IGet<DeploymentProcessResource>, IModify<DeploymentProcessResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<ReleaseTemplateResource> GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel);
        Task<ReleaseTemplateResource> GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel, CancellationToken cancellationToken);

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<DeploymentProcessResource> Get(ProjectResource projectResource, string gitRef);
        Task<DeploymentProcessResource> Get(ProjectResource projectResource, string gitRef, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<DeploymentProcessResource> Get(ProjectResource projectResource);
        Task<DeploymentProcessResource> Get(ProjectResource projectResource, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<DeploymentProcessResource> Modify(DeploymentProcessResource deploymentSettings, string commitMessage);
        Task<DeploymentProcessResource> Modify(DeploymentProcessResource deploymentSettings, string commitMessage, CancellationToken cancellationToken);
    }

    class DeploymentProcessRepository : BasicRepository<DeploymentProcessResource>, IDeploymentProcessRepository
    {

        public DeploymentProcessRepository(IOctopusAsyncRepository repository)
            : base(repository, "DeploymentProcesses")
        {
        }

        public Task<ReleaseTemplateResource> GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel)
        {
            return GetTemplate(deploymentProcess, channel, CancellationToken.None);
        }
        
        public Task<ReleaseTemplateResource> GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel, CancellationToken cancellationToken)
        {
            return Client.Get<ReleaseTemplateResource>(deploymentProcess.Link("Template"), new {channel = channel?.Id}, cancellationToken);
        }

        public Task<DeploymentProcessResource> Get(ProjectResource projectResource, string gitRef)
        {
            return Get(projectResource, gitRef, CancellationToken.None);
        }
        
        public Task<DeploymentProcessResource> Get(ProjectResource projectResource, string gitRef, CancellationToken cancellationToken)
        {
            if (!projectResource.IsVersionControlled)
            {
                throw new NotSupportedException(
                    $"Database backed projects require using the overload that does not include a gitRef parameter.");
            }

            return Client.Get<DeploymentProcessResource>(projectResource.Link("DeploymentProcess"), new {gitRef}, cancellationToken);
        }

        public Task<DeploymentProcessResource> Get(ProjectResource projectResource)
        {
            return Get(projectResource, CancellationToken.None);
        }
        
        public Task<DeploymentProcessResource> Get(ProjectResource projectResource, CancellationToken cancellationToken)
        {
            if (projectResource.PersistenceSettings is GitPersistenceSettingsResource vcsResource)
            {
                return Get(projectResource, vcsResource.DefaultBranch, cancellationToken);
            }

            return Client.Get<DeploymentProcessResource>(projectResource.Link("DeploymentProcess"), cancellationToken);
        }

        public async Task<DeploymentProcessResource> Modify(DeploymentProcessResource deploymentSettings, string commitMessage)
        {
            return await Modify(deploymentSettings, CancellationToken.None);
        }
        
        public async Task<DeploymentProcessResource> Modify(DeploymentProcessResource deploymentSettings, string commitMessage, CancellationToken cancellationToken)
        {
            var firstCaCVersion = new SemanticVersion(2021, 3, 2066);
            await EnsureServerIsMinimumVersion(
                firstCaCVersion,
                currentServerVersion => $"The version of the Octopus Server ('{currentServerVersion}') you are connecting to is not compatible with this version of Octopus.Client for this API call. Please upgrade your Octopus Server to a version greater than '{firstCaCVersion}'",
                cancellationToken
            );
            
            
            // TODO: revisit/obsolete this API when we have converters
            // until then we need a way to re-use the response from previous client calls
            var json = Serializer.Serialize(deploymentSettings);
            var command = Serializer.Deserialize<ModifyDeploymentProcessCommand>(json);

            command.ChangeDescription = commitMessage;

            return await Modify(command, cancellationToken);
        }
    }
}
