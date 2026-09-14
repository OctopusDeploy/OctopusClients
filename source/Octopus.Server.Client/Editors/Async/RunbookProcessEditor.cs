using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class RunbookProcessEditor : IResourceEditor<RunbookProcessResource, RunbookProcessEditor>
    {
        private readonly IRunbookProcessRepository repository;

        public RunbookProcessEditor(IRunbookProcessRepository repository)
        {
            this.repository = repository;
        }

        public RunbookProcessResource Instance { get; private set; }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<RunbookProcessEditor> Load(string id)
            => Load(id, CancellationToken.None);

        public async Task<RunbookProcessEditor> Load(string id, CancellationToken cancellationToken)
        {
            Instance = await repository.Get(id, cancellationToken).ConfigureAwait(false);
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

        public RunbookProcessEditor RemoveStep(string name)
        {
            Instance.RemoveStep(name);
            return this;
        }

        public RunbookProcessEditor ClearSteps()
        {
            Instance.ClearSteps();
            return this;
        }

        public RunbookProcessEditor Customize(Action<RunbookProcessResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<RunbookProcessEditor> Save()
            => Save(CancellationToken.None);

        public async Task<RunbookProcessEditor> Save(CancellationToken cancellationToken)
        {
            Instance = await repository.Modify(Instance, cancellationToken).ConfigureAwait(false);
            return this;
        }
    }
}
