using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class DeploymentProcessEditor : IResourceEditor<DeploymentProcessResource, DeploymentProcessEditor>
    {
        private readonly IDeploymentProcessRepository repository;

        public DeploymentProcessEditor(IDeploymentProcessRepository repository)
        {
            this.repository = repository;
        }

        public DeploymentProcessResource Instance { get; private set; }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<DeploymentProcessEditor> Load(string id)
            => Load(id, CancellationToken.None);

        public async Task<DeploymentProcessEditor> Load(string id, CancellationToken cancellationToken)
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

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<DeploymentProcessEditor> Save()
            => Save(CancellationToken.None);

        public async Task<DeploymentProcessEditor> Save(CancellationToken cancellationToken)
        {
            Instance = await repository.Modify(Instance, cancellationToken).ConfigureAwait(false);
            return this;
        }
    }
}
