using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class RunbookEditor : IResourceEditor<RunbookResource, RunbookEditor>
    {
        private readonly IRunbookRepository repository;
        private readonly Lazy<RunbookProcessEditor> runbookProcess;

        public RunbookEditor(IRunbookRepository repository,
            IRunbookProcessRepository runbookProcessRepository)
        {
            this.repository = repository;
            runbookProcess = new Lazy<RunbookProcessEditor>(() => new RunbookProcessEditor(runbookProcessRepository).Load(Instance.RunbookProcessId));
        }

        public RunbookResource Instance { get; private set; }

        public RunbookProcessEditor RunbookProcess => runbookProcess.Value;

        public RunbookEditor CreateOrModify(ProjectResource project, string name, string description)
        {
            var existing = repository.FindByName(project, name);

            if (existing == null)
            {
                Instance = repository.Create(new RunbookResource
                {
                    ProjectId = project.Id,
                    Name = name,
                    Description = description,
                    EnvironmentScope = RunbookEnvironmentScope.All,
                    DefaultGuidedFailureMode = GuidedFailureMode.EnvironmentDefault,
                    RunRetentionPolicy = new RunbookRetentionPeriod(100)
                });
            }
            else
            {
                existing.Name = name;
                existing.Description = description;

                Instance = repository.Modify(existing);
            }

            return this;
        }

        public RunbookEditor Load(string id)
        {
            Instance = repository.Get(id);
            return this;
        }

        public RunbookEditor Customize(Action<RunbookResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public RunbookEditor Save()
        {
            Instance = repository.Modify(Instance);
            if (runbookProcess.IsValueCreated)
            {
                runbookProcess.Value.Save();
            }
            return this;
        }
    }
}