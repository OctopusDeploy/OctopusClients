using System;
using System.IO;
using Octopus.Client.Editors;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class ProjectEditor : IResourceEditor<ProjectResource, ProjectEditor>
    {
        private readonly IProjectRepository repository;
        private readonly Lazy<ProjectChannelsEditor> channels;
        private readonly Lazy<DeploymentProcessEditor> deploymentProcess;
        private readonly Lazy<ProjectTriggersEditor> triggers;
        private readonly Lazy<VariableSetEditor> variables;

        public ProjectEditor(
            IProjectRepository repository,
            IChannelRepository channelRepository,
            IDeploymentProcessRepository deploymentProcessRepository,
            IProjectTriggerRepository projectTriggerRepository,
            IVariableSetRepository variableSetRepository)
        {
            this.repository = repository;
            channels = new Lazy<ProjectChannelsEditor>(() => new ProjectChannelsEditor(channelRepository, Instance));
            deploymentProcess = new Lazy<DeploymentProcessEditor>(() => new DeploymentProcessEditor(deploymentProcessRepository).Load(Instance.DeploymentProcessId));
            triggers = new Lazy<ProjectTriggersEditor>(() => new ProjectTriggersEditor(projectTriggerRepository, Instance));
            variables = new Lazy<VariableSetEditor>(() => new VariableSetEditor(variableSetRepository).Load(Instance.VariableSetId));
        }

        public ProjectResource Instance { get; private set; }

        public ProjectChannelsEditor Channels => channels.Value;

        public DeploymentProcessEditor DeploymentProcess => deploymentProcess.Value;

        public ProjectTriggersEditor Triggers => triggers.Value;

        public VariableSetEditor Variables => variables.Value;

        public IVariableTemplateContainerEditor<ProjectResource> VariableTemplates => Instance;

        public ProjectEditor CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle)
        {
            var existing = repository.FindByName(name);

            if (existing == null)
            {
                Instance = repository.Create(new ProjectResource
                {
                    Name = name,
                    ProjectGroupId = projectGroup.Id,
                    LifecycleId = lifecycle.Id,
                });
            }
            else
            {
                existing.Name = name;
                existing.ProjectGroupId = projectGroup.Id;
                existing.LifecycleId = lifecycle.Id;

                Instance = repository.Modify(existing);
            }

            return this;
        }

        public ProjectEditor CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description, string cloneId = null)
        {
            var existing = repository.FindByName(name);

            if (existing == null)
            {
                Instance = repository.Create(new ProjectResource
                {
                    Name = name,
                    ProjectGroupId = projectGroup.Id,
                    LifecycleId = lifecycle.Id,
                    Description = description
                }, new { clone = cloneId });
            }
            else
            {
                existing.Name = name;
                existing.ProjectGroupId = projectGroup.Id;
                existing.LifecycleId = lifecycle.Id;
                existing.Description = description;

                Instance = repository.Modify(existing);
            }

            return this;
        }

        public ProjectEditor SetLogo(string logoFilePath)
        {
            using (var stream = new FileStream(logoFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                repository.SetLogo(Instance, Path.GetFileName(logoFilePath), stream);
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

        public ProjectEditor Save()
        {
            Instance = repository.Modify(Instance);
            if (channels.IsValueCreated)
            {
                channels.Value.SaveAll();
            }
            if (deploymentProcess.IsValueCreated)
            {
                deploymentProcess.Value.Save();
            }
            if (triggers.IsValueCreated)
            {
                triggers.Value.SaveAll();
            }
            if (variables.IsValueCreated)
            {
                variables.Value.Save();
            }
            return this;
        }
    }
}