using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class RunbookEditor : IResourceEditor<RunbookResource, RunbookEditor>
    {
        private readonly IRunbookRepository repository;
        private readonly Lazy<Task<RunbookStepsEditor>> runbookSteps;

        public RunbookEditor(IRunbookRepository repository,
            IRunbookStepsRepository runbookStepsRepository)
        {
            this.repository = repository;
            runbookSteps = new Lazy<Task<RunbookStepsEditor>>(() => new RunbookStepsEditor(runbookStepsRepository).Load(Instance.RunbookStepsId));
        }

        public RunbookResource Instance { get; private set; }

        public Task<RunbookStepsEditor> RunbookSteps => runbookSteps.Value;

        public async Task<RunbookEditor> CreateOrModify(ProjectResource project, string name, string description)
        {
            var existing = await repository.FindByName(project, name).ConfigureAwait(false);

            if (existing == null)
            {
                Instance = await repository.Create(new RunbookResource
                {
                    ProjectId = project.Id,
                    Name = name,
                    Description = description
                }).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Description = description;

                Instance = await repository.Modify(existing).ConfigureAwait(false);
            }

            return this;
        }

        public async Task<RunbookEditor> Load(string id)
        {
            Instance = await repository.Get(id).ConfigureAwait(false);
            return this;
        }

        public RunbookEditor Customize(Action<RunbookResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<RunbookEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            if (runbookSteps.IsValueCreated)
            {
                var steps = await runbookSteps.Value.ConfigureAwait(false);
                await steps.Save().ConfigureAwait(false);
            }
            return this;
        }
    }
}