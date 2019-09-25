using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class RunbookStepsEditor : IResourceEditor<RunbookStepsResource, RunbookStepsEditor>
    {
        private readonly IRunbookStepsRepository repository;

        public RunbookStepsEditor(IRunbookStepsRepository repository)
        {
            this.repository = repository;
        }

        public RunbookStepsResource Instance { get; private set; }

        public async Task<RunbookStepsEditor> Load(string id)
        {
            Instance = await repository.Get(id).ConfigureAwait(false);
            return this;
        }

        public DeploymentStepResource FindStep(string name)
        {
            return Instance.FindStep(name);
        }

        public DeploymentStepResource AddOrUpdateStep(string name)
        {
            return Instance.AddOrUpdateStep(name);
        }

        public RunbookStepsEditor RemoveStep(string name)
        {
            Instance.RemoveStep(name);
            return this;
        }

        public RunbookStepsEditor ClearSteps()
        {
            Instance.ClearSteps();
            return this;
        }

        public RunbookStepsEditor Customize(Action<RunbookStepsResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<RunbookStepsEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}