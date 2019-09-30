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

        public async Task<RunbookEditor> CreateOrModify(string projectId, string name)
        {
            var existing = await repository.FindByName(name);

            if (existing == null)
            {
                Instance = await repository.Create(new RunbookResource
                {
                    Name = name,
                    ProjectId = projectId,
                });
            }
            else
            {
                existing.Name = name;
                Instance = await repository.Modify(existing);
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
            return this;
        }
    }
}