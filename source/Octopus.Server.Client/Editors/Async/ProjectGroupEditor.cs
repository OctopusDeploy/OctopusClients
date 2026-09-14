using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class ProjectGroupEditor : IResourceEditor<ProjectGroupResource, ProjectGroupEditor>
    {
        private readonly IProjectGroupRepository repository;

        public ProjectGroupEditor(IProjectGroupRepository repository)
        {
            this.repository = repository;
        }

        public ProjectGroupResource Instance { get; private set; }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ProjectGroupEditor> CreateOrModify(string name)
            => CreateOrModify(name, CancellationToken.None);

        public async Task<ProjectGroupEditor> CreateOrModify(string name, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new ProjectGroupResource
                {
                    Name = name,
                }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;

                Instance = await repository.Modify(existing, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ProjectGroupEditor> CreateOrModify(string name, string description)
            => CreateOrModify(name, description, CancellationToken.None);

        public async Task<ProjectGroupEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);
            if (existing == null)
            {
                Instance = await repository.Create(new ProjectGroupResource
                {
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

        public ProjectGroupEditor Customize(Action<ProjectGroupResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<ProjectGroupEditor> Save()
            => Save(CancellationToken.None);

        public async Task<ProjectGroupEditor> Save(CancellationToken cancellationToken)
        {
            Instance = await repository.Modify(Instance, cancellationToken).ConfigureAwait(false);
            return this;
        }
    }
}
