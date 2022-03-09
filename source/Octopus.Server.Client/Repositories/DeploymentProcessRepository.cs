using System;
using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Repositories
{
    public interface IDeploymentProcessRepository : IGet<DeploymentProcessResource>, IModify<DeploymentProcessResource>
    {
        ReleaseTemplateResource GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel);
        DeploymentProcessResource Get(ProjectResource projectResource);
        DeploymentProcessResource Get(ProjectResource projectResource, string gitRef);
        DeploymentProcessResource Modify(DeploymentProcessResource deploymentSettings, string commitMessage);
    }
    

    class DeploymentProcessRepository : BasicRepository<DeploymentProcessResource>, IDeploymentProcessRepository
    {

        public DeploymentProcessRepository(IOctopusRepository repository)
            : base(repository, "DeploymentProcesses")
        {
        }

        public ReleaseTemplateResource GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel)
        {
            return Client.Get<ReleaseTemplateResource>(deploymentProcess.Link("Template"), new {channel = channel?.Id});
        }

        public DeploymentProcessResource Get(ProjectResource projectResource)
        {
            if (projectResource.PersistenceSettings is GitPersistenceSettingsResource vcsResource)
            {
               return Get(projectResource, vcsResource.DefaultBranch);
            }
            
            return Client.Get<DeploymentProcessResource>(projectResource.Link("DeploymentProcess"));
        }

        public DeploymentProcessResource Get(ProjectResource projectResource, string gitRef)
        {
            if (!projectResource.IsVersionControlled)
            {
                throw new NotSupportedException(
                    $"Database backed projects require using the overload that does not include a gitRef parameter.");
            }

            return Client.Get<DeploymentProcessResource>(projectResource.Link("DeploymentProcess"), new { gitRef });
        }

        public DeploymentProcessResource Modify(DeploymentProcessResource deploymentSettings, string commitMessage)
        {
            // TODO: revisit/obsolete this API when we have converters
            // until then we need a way to re-use the response from previous client calls
            var json = Serializer.Serialize(deploymentSettings);
            var command = Serializer.Deserialize<ModifyDeploymentProcessCommand>(json);

            command.ChangeDescription = commitMessage;

            return Modify(command);
        }
    }
}