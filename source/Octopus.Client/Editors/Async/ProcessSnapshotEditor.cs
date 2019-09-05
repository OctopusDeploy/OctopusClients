using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class ProcessSnapshotEditor : IResourceEditor<DeploymentProcessResource, ProcessSnapshotEditor>
    {
        private readonly IProcessSnapshotRepository repository;

        public ProcessSnapshotEditor(IProcessSnapshotRepository repository)
        {
            this.repository = repository;
        }

        public DeploymentProcessResource Instance { get; private set; }

        public async Task<ProcessSnapshotEditor> Load(string id)
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

        public async Task<ProcessSnapshotEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}