using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class RunbookEditor : IResourceEditor<RunbookResource, RunbookEditor>
    {
        private readonly IRunbookRepository repository;
        private readonly Lazy<Task<RunbookProcessEditor>> runbookProcess;

        public RunbookEditor(IRunbookRepository repository,
            IRunbookProcessRepository runbookProcessRepository)
        {
            this.repository = repository;
            runbookProcess = new Lazy<Task<RunbookProcessEditor>>(() => new RunbookProcessEditor(runbookProcessRepository).Load(Instance.RunbookProcessId, CancellationToken.None));
        }

        public RunbookResource Instance { get; private set; }

        public Task<RunbookProcessEditor> RunbookProcess => runbookProcess.Value;

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<RunbookEditor> CreateOrModify(ProjectResource project, string name, string description)
            => CreateOrModify(project, name, description, CancellationToken.None);

        public async Task<RunbookEditor> CreateOrModify(ProjectResource project, string name, string description, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(project, name, cancellationToken).ConfigureAwait(false);

            if (existing == null)
            {
                Instance = await repository.Create(new RunbookResource
                {
                    ProjectId = project.Id,
                    Name = name,
                    Description = description
                }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Description = description;

                Instance = await repository.Modify(existing, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<RunbookEditor> Load(string id)
            => Load(id, CancellationToken.None);

        public async Task<RunbookEditor> Load(string id, CancellationToken cancellationToken)
        {
            Instance = await repository.Get(id, cancellationToken).ConfigureAwait(false);
            return this;
        }

        public RunbookEditor Customize(Action<RunbookResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<RunbookEditor> Save()
            => Save(CancellationToken.None);

        public async Task<RunbookEditor> Save(CancellationToken cancellationToken)
        {
            Instance = await repository.Modify(Instance, cancellationToken).ConfigureAwait(false);
            if (runbookProcess.IsValueCreated)
            {
                var steps = await runbookProcess.Value.ConfigureAwait(false);
                await steps.Save(cancellationToken).ConfigureAwait(false);
            }
            return this;
        }
    }
}
