using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class OpsStepsEditor : IResourceEditor<StepsResource, OpsStepsEditor>
    {
        private readonly IOpsStepsRepository repository;

        public OpsStepsEditor(IOpsStepsRepository repository)
        {
            this.repository = repository;
        }

        public StepsResource Instance { get; private set; }

        public async Task<OpsStepsEditor> Load(string id)
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

        public OpsStepsEditor RemoveStep(string name)
        {
            Instance.RemoveStep(name);
            return this;
        }

        public OpsStepsEditor ClearSteps()
        {
            Instance.ClearSteps();
            return this;
        }

        public OpsStepsEditor Customize(Action<StepsResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<OpsStepsEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}