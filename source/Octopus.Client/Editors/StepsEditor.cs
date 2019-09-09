using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class StepsEditor : IResourceEditor<StepsResource, StepsEditor>
    {
        private readonly IStepsRepository repository;

        public StepsEditor(IStepsRepository repository)
        {
            this.repository = repository;
        }

        public StepsResource Instance { get; private set; }

        public StepsEditor Load(string id)
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

        public StepsEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}