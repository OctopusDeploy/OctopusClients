using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class ProcessSnapshotEditor : IResourceEditor<DeploymentProcessResource, ProcessSnapshotEditor>
    {
        private readonly IProcessSnapshotRepository repository;

        public ProcessSnapshotEditor(IProcessSnapshotRepository repository)
        {
            this.repository = repository;
        }

        public DeploymentProcessResource Instance { get; private set; }

        public ProcessSnapshotEditor Load(string id)
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

        public ProcessSnapshotEditor RemoveStep(string name)
        {
            Instance.RemoveStep(name);
            return this;
        }

        public ProcessSnapshotEditor ClearSteps()
        {
            Instance.ClearSteps();
            return this;
        }

        public ProcessSnapshotEditor Customize(Action<DeploymentProcessResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public ProcessSnapshotEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}