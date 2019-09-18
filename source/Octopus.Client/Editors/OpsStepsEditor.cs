using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class OpsStepsEditor : IResourceEditor<StepsResource, OpsStepsEditor>
    {
        private readonly IOpsStepsRepository repository;

        public OpsStepsEditor(IOpsStepsRepository repository)
        {
            this.repository = repository;
        }

        public StepsResource Instance { get; private set; }

        public OpsStepsEditor Load(string id)
        {
            Instance = repository.Get(id);
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

        public OpsStepsEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}