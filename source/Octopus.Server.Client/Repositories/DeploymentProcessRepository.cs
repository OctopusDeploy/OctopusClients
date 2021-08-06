using Octopus.Client.Model;
using Octopus.Client.Serialization;

namespace Octopus.Client.Repositories
{
    public interface IDeploymentProcessRepository : IGet<DeploymentProcessResource>, IModify<DeploymentProcessResource>
    {
        IDeploymentProcessRepositoryBeta Beta();
        ReleaseTemplateResource GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel);
    }

    class DeploymentProcessRepository : BasicRepository<DeploymentProcessResource>, IDeploymentProcessRepository
    {
        private readonly DeploymentProcessRepositoryBeta beta;

        public DeploymentProcessRepository(IOctopusRepository repository)
            : base(repository, "DeploymentProcesses")
        {
            beta = new DeploymentProcessRepositoryBeta(repository);
        }

        public IDeploymentProcessRepositoryBeta Beta()
        {
            return beta;
        }

        public ReleaseTemplateResource GetTemplate(DeploymentProcessResource deploymentProcess, ChannelResource channel)
        {
            return Client.Get<ReleaseTemplateResource>(deploymentProcess.Link("Template"), new { channel = channel?.Id });
        }
    }

    public interface IDeploymentProcessRepositoryBeta
    {
        DeploymentProcessResource Get(ProjectResource projectResource, string gitRef = null);
        DeploymentProcessResource Modify(ProjectResource projectResource, DeploymentProcessResource resource, string commitMessage = null);
        DeploymentProcessResource Modify(ProjectResource projectResource, ModifyDeploymentProcessCommand command);
    }

    class DeploymentProcessRepositoryBeta : IDeploymentProcessRepositoryBeta
    {
        private readonly IOctopusRepository repository;
        private readonly IOctopusClient client;

        public DeploymentProcessRepositoryBeta(IOctopusRepository repository)
        {
            this.repository = repository;
            client = repository.Client;
        }

        public DeploymentProcessResource Get(ProjectResource projectResource, string gitRef = null)
        {
            if (!(projectResource.PersistenceSettings is VersionControlSettingsResource settings))
                return repository.DeploymentProcesses.Get(projectResource.DeploymentProcessId);

            gitRef = gitRef ?? settings.DefaultBranch;

            return client.Get<DeploymentProcessResource>(projectResource.Link("DeploymentProcess"), new { gitRef });
        }

        public DeploymentProcessResource Modify(ProjectResource projectResource, DeploymentProcessResource resource, string commitMessage = null)
        {
            // TODO: revisit/obsolete this API when we have converters
            // until then we need a way to re-use the response from previous client calls
            var json = Serializer.Serialize(resource);
            var command = Serializer.Deserialize<ModifyDeploymentProcessCommand>(json);
            
            command.ChangeDescription = commitMessage;
            
            return Modify(projectResource, command);
        }

        public DeploymentProcessResource Modify(ProjectResource projectResource, ModifyDeploymentProcessCommand command)
        {
            if (!projectResource.IsVersionControlled)
            {
                return client.Update(projectResource.Link("DeploymentProcess"), command);
            }

            client.Put(command.Link("Self"), command);
            return client.Get<DeploymentProcessResource>(command.Link("Self"));
        }
    }
}