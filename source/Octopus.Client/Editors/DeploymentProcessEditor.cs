using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class DeploymentProcessEditor : IResourceEditor<DeploymentProcessResource, DeploymentProcessEditor>
    {
        private readonly IDeploymentProcessRepository repository;

        public DeploymentProcessEditor(IDeploymentProcessRepository repository)
        {
            this.repository = repository;
        }

        public DeploymentProcessResource Instance { get; private set; }

        public DeploymentProcessEditor Load(string id)
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

        public DeploymentProcessEditor RemoveStep(string name)
        {
            Instance.RemoveStep(name);
            return this;
        }

        public DeploymentProcessEditor ClearSteps()
        {
            Instance.ClearSteps();
            return this;
        }

        public DeploymentProcessEditor Customize(Action<DeploymentProcessResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public DeploymentProcessEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}