using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Repositories.Async
{
    public interface IDeploymentProcessRepository : IGet<DeploymentProcessResource>, IModify<DeploymentProcessResource>
    {
        Task<ReleaseTemplateResource> GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel);

        Task<DeploymentProcessResource> Get(ProjectResource projectResource, string gitRef);
        Task<DeploymentProcessResource> Get(ProjectResource projectResource);
        Task<DeploymentProcessResource> Modify(DeploymentProcessResource deploymentSettings, string commitMessage);
    }

    class DeploymentProcessRepository : BasicRepository<DeploymentProcessResource>, IDeploymentProcessRepository
    {

        public DeploymentProcessRepository(IOctopusAsyncRepository repository)
            : base(repository, "DeploymentProcesses")
        {
        }

        public Task<ReleaseTemplateResource> GetTemplate(DeploymentProcessResource deploymentProcess,
            ChannelResource channel)
        {
            return Client.Get<ReleaseTemplateResource>(deploymentProcess.Link("Template"), new {channel = channel?.Id});
        }

        public Task<DeploymentProcessResource> Get(ProjectResource projectResource, string gitRef)
        {
            if (!projectResource.IsVersionControlled)
            {
                throw new NotSupportedException(
                    $"Database backed projects require using the overload that does not include a gitRef parameter.");
            }

            return Client.Get<DeploymentProcessResource>(projectResource.Link(this.CollectionLinkName), new {gitRef});
        }

        public Task<DeploymentProcessResource> Get(ProjectResource projectResource)
        {
            if (projectResource.PersistenceSettings is VersionControlSettingsResource vcsResource)
            {
                return Get(projectResource, vcsResource.DefaultBranch);
            }

            return Client.Get<DeploymentProcessResource>(projectResource.Link(this.CollectionLinkName));
        }

        public async Task<DeploymentProcessResource> Modify(DeploymentProcessResource deploymentSettings,
            string commitMessage)
        {
            var firstCaCVersion = new SemanticVersion(2021, 3, 2066);
            await EnsureServerIsMinimumVersion(
                firstCaCVersion,
                currentServerVersion => $"The version of the Octopus Server ('{currentServerVersion}') you are connecting to is not compatible with this version of Octopus.Client for this API call. Please upgrade your Octopus Server to a version greater than '{firstCaCVersion}'");
            
            
            // TODO: revisit/obsolete this API when we have converters
            // until then we need a way to re-use the response from previous client calls
            var json = Serializer.Serialize(deploymentSettings);
            var command = Serializer.Deserialize<ModifyDeploymentProcessCommand>(json);

            command.ChangeDescription = commitMessage;

            return await Modify(command);
        }
    }
}
