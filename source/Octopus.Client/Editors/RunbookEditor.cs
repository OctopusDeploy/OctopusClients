using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class RunbookEditor : IResourceEditor<RunbookResource, RunbookEditor>
    {
        private readonly IRunbookRepository repository;
        private readonly Lazy<RunbookStepsEditor> runbookSteps;

        public RunbookEditor(IRunbookRepository repository,
            IRunbookStepsRepository runbookStepsRepository)
        {
            this.repository = repository;
            runbookSteps = new Lazy<RunbookStepsEditor>(() => new RunbookStepsEditor(runbookStepsRepository).Load(Instance.RunbookStepsId));
        }

        public RunbookResource Instance { get; private set; }

        public RunbookStepsEditor RunbookSteps => runbookSteps.Value;

        public RunbookEditor CreateOrModify(string projectId, string name)
        {
            var existing = repository.FindByName(name);

            if (existing == null)
            {
                Instance = repository.Create(new RunbookResource
                {
                    Name = name,
                    ProjectId = projectId,
                });
            }
            else
            {
                existing.Name = name;
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
            if (runbookSteps.IsValueCreated)
            {
                runbookSteps.Value.Save();
            }
            return this;
        }
    }
}