using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class ProjectEditor : IResourceEditor<ProjectResource, ProjectEditor>
    {
        private readonly IProjectRepository repository;
        private readonly Lazy<ProjectChannelsEditor> channels;
        private readonly Lazy<Task<DeploymentProcessEditor>> deploymentProcess;
        private readonly Lazy<ProjectTriggersEditor> triggers;
        private readonly Lazy<Task<VariableSetEditor>> variables;

        public ProjectEditor(
            IProjectRepository repository,
            IChannelRepository channelRepository,
            IDeploymentProcessRepository deploymentProcessRepository,
            IProjectTriggerRepository projectTriggerRepository,
            IVariableSetRepository variableSetRepository)
        {
            this.repository = repository;
            channels = new Lazy<ProjectChannelsEditor>(() => new ProjectChannelsEditor(channelRepository, Instance));
            deploymentProcess = new Lazy<Task<DeploymentProcessEditor>>(() => new DeploymentProcessEditor(deploymentProcessRepository).Load(Instance.DeploymentProcessId, CancellationToken.None));
            triggers = new Lazy<ProjectTriggersEditor>(() => new ProjectTriggersEditor(projectTriggerRepository, Instance));
            variables = new Lazy<Task<VariableSetEditor>>(() => new VariableSetEditor(variableSetRepository).Load(Instance.VariableSetId, CancellationToken.None));
        }

        public ProjectResource Instance { get; private set; }

        public ProjectChannelsEditor Channels => channels.Value;

        public Task<DeploymentProcessEditor> DeploymentProcess => deploymentProcess.Value;

        public ProjectTriggersEditor Triggers => triggers.Value;

        public Task<VariableSetEditor> Variables => variables.Value;

        public IVariableTemplateContainerEditor<ProjectResource> VariableTemplates => Instance;

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle)
            => CreateOrModify(name, projectGroup, lifecycle, CancellationToken.None);

        public async Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);

            if (existing == null)
            {
                Instance = await repository.Create(new ProjectResource
                {
                    Name = name,
                    ProjectGroupId = projectGroup.Id,
                    LifecycleId = lifecycle.Id,
                }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.ProjectGroupId = projectGroup.Id;
                existing.LifecycleId = lifecycle.Id;

                Instance = await repository.Modify(existing, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        public async Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description, string cloneId = null) => await CreateOrModify(name, projectGroup, lifecycle, description, cloneId, CancellationToken.None);
        public async Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description, CancellationToken cancellationToken) => await CreateOrModify(name, projectGroup, lifecycle, description, cloneId: null, cancellationToken);

        public async Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description, string cloneId, CancellationToken cancellationToken)
            => await CreateOrModify(name, projectGroup, lifecycle, description, cloneId, retainTenantConnections: false, cancellationToken).ConfigureAwait(false);

        public async Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description, string cloneId, bool retainTenantConnections, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);

            if (existing == null)
            {
                Instance = await repository.Create(new ProjectResource
                {
                    Name = name,
                    ProjectGroupId = projectGroup.Id,
                    LifecycleId = lifecycle.Id,
                    Description = description,
                    RetainTenantConnections = retainTenantConnections
                }, new { clone = cloneId }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.ProjectGroupId = projectGroup.Id;
                existing.LifecycleId = lifecycle.Id;
                existing.Description = description;

                Instance = await repository.Modify(existing, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ProjectEditor> SetLogo(string logoFilePath)
            => SetLogo(logoFilePath, CancellationToken.None);

        public async Task<ProjectEditor> SetLogo(string logoFilePath, CancellationToken cancellationToken)
        {
            using (var stream = new FileStream(logoFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await repository.SetLogo(Instance, Path.GetFileName(logoFilePath), stream, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        public ProjectEditor IncludingLibraryVariableSets(params LibraryVariableSetResource[] libraryVariableSets)
        {
            Instance.IncludingLibraryVariableSets(libraryVariableSets);
            return this;
        }

        public ProjectEditor Customize(Action<ProjectResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ProjectEditor> Save()
            => Save(CancellationToken.None);

        public async Task<ProjectEditor> Save(CancellationToken cancellationToken)
        {
            Instance = await repository.Modify(Instance, cancellationToken).ConfigureAwait(false);
            if (channels.IsValueCreated)
            {
                await channels.Value.SaveAll(cancellationToken).ConfigureAwait(false);
            }
            if (deploymentProcess.IsValueCreated)
            {
                var depProcess = await deploymentProcess.Value.ConfigureAwait(false);
                await depProcess.Save(cancellationToken).ConfigureAwait(false);
            }
            if (triggers.IsValueCreated)
            {
                await triggers.Value.SaveAll(cancellationToken).ConfigureAwait(false);
            }
            if (variables.IsValueCreated)
            {
                var vars = await variables.Value.ConfigureAwait(false);
                await vars.Save(cancellationToken).ConfigureAwait(false);
            }
            return this;
        }
    }
}
