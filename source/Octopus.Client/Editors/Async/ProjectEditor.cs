using System;
using System.IO;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class ProjectEditor : IResourceEditor<ProjectResource, ProjectEditor>
    {
        private readonly IProjectRepository repository;
        private readonly Lazy<ProjectChannelsEditor> channels;
        private readonly Lazy<Task<ProcessEditor>> process;
        private readonly Lazy<Task<StepsEditor>> steps;
        private readonly Lazy<ProjectTriggersEditor> triggers; 
        private readonly Lazy<Task<VariableSetEditor>> variables;

        public ProjectEditor(
            IProjectRepository repository,
            IChannelRepository channelRepository,
            IProcessRepository processRepository,
            IStepsRepository stepsRepository,
            IProjectTriggerRepository projectTriggerRepository,
            IVariableSetRepository variableSetRepository)
        {
            this.repository = repository;
            channels = new Lazy<ProjectChannelsEditor>(() => new ProjectChannelsEditor(channelRepository, Instance));
            process = new Lazy<Task<ProcessEditor>>(() => new ProcessEditor(processRepository).Load(Instance.ProcessId));
            steps = new Lazy<Task<StepsEditor>>(() => new StepsEditor(stepsRepository).Load(process.Value.Result.Instance.StepsId));
            triggers = new Lazy<ProjectTriggersEditor>(() => new ProjectTriggersEditor(projectTriggerRepository, Instance));
            variables = new Lazy<Task<VariableSetEditor>>(() => new VariableSetEditor(variableSetRepository).Load(Instance.VariableSetId));
        }

        public ProjectResource Instance { get; private set; }

        public ProjectChannelsEditor Channels => channels.Value;

        public Task<StepsEditor> DeploymentProcess => steps.Value;

        public ProjectTriggersEditor Triggers => triggers.Value;

        public Task<VariableSetEditor> Variables => variables.Value;

        public IVariableTemplateContainerEditor<ProjectResource> VariableTemplates => Instance;

        public async Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle)
        {
            var existing = await repository.FindByName(name).ConfigureAwait(false);

            if (existing == null)
            {
                Instance = await repository.Create(new ProjectResource
                {
                    Name = name,
                    ProjectGroupId = projectGroup.Id,
                    LifecycleId = lifecycle.Id,
                }).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.ProjectGroupId = projectGroup.Id;
                existing.LifecycleId = lifecycle.Id;

                Instance = await repository.Modify(existing).ConfigureAwait(false);
            }

            return this;
        }

        public async Task<ProjectEditor> CreateOrModify(string name, ProjectGroupResource projectGroup, LifecycleResource lifecycle, string description, string cloneId = null)
        {
            var existing = await repository.FindByName(name).ConfigureAwait(false);

            if (existing == null)
            {
                Instance = await repository.Create(new ProjectResource
                {
                    Name = name,
                    ProjectGroupId = projectGroup.Id,
                    LifecycleId = lifecycle.Id,
                    Description = description
                }, new { clone = cloneId }).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.ProjectGroupId = projectGroup.Id;
                existing.LifecycleId = lifecycle.Id;
                existing.Description = description;

                Instance = await repository.Modify(existing).ConfigureAwait(false);
            }

            return this;
        }

        public async Task<ProjectEditor> SetLogo(string logoFilePath)
        {
            using (var stream = new FileStream(logoFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await repository.SetLogo(Instance, Path.GetFileName(logoFilePath), stream).ConfigureAwait(false);
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

        public async Task<ProjectEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            if (channels.IsValueCreated)
            {
                await channels.Value.SaveAll().ConfigureAwait(false);
            }
            if (steps.IsValueCreated)
            {
                var depProcess = await steps.Value.ConfigureAwait(false);
                await depProcess.Save().ConfigureAwait(false);
            }
            if (triggers.IsValueCreated)
            {
                await triggers.Value.SaveAll().ConfigureAwait(false);
            }
            if (variables.IsValueCreated)
            {
                var vars = await variables.Value.ConfigureAwait(false);
                await vars.Save().ConfigureAwait(false);
            }
            return this;
        }
    }
}