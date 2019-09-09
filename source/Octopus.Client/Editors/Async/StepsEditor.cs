using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class StepsEditor : IResourceEditor<StepsResource, StepsEditor>
    {
        private readonly IStepsRepository repository;

        public StepsEditor(IStepsRepository repository)
        {
            this.repository = repository;
        }

        public StepsResource Instance { get; private set; }

        public async Task<StepsEditor> Load(string id)
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

        public StepsEditor RemoveStep(string name)
        {
            Instance.RemoveStep(name);
            return this;
        }

        public StepsEditor ClearSteps()
        {
            Instance.ClearSteps();
            return this;
        }

        public StepsEditor Customize(Action<StepsResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<StepsEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}