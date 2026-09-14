using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class LibraryVariableSetEditor : IResourceEditor<LibraryVariableSetResource, LibraryVariableSetEditor>
    {
        private readonly ILibraryVariableSetRepository repository;
        private readonly Lazy<Task<VariableSetEditor>> variables;

        public LibraryVariableSetEditor(ILibraryVariableSetRepository repository, IVariableSetRepository variableSetRepository)
        {
            this.repository = repository;
            variables = new Lazy<Task<VariableSetEditor>>(() => new VariableSetEditor(variableSetRepository).Load(Instance.VariableSetId, CancellationToken.None));
        }

        public LibraryVariableSetResource Instance { get; private set; }

        public Task<VariableSetEditor> Variables => variables.Value;

        public IVariableTemplateContainerEditor<LibraryVariableSetResource> VariableTemplates => Instance;

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<LibraryVariableSetEditor> CreateOrModify(string name)
            => CreateOrModify(name, CancellationToken.None);

        public async Task<LibraryVariableSetEditor> CreateOrModify(string name, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);

            if (existing == null)
            {
                Instance = await repository.Create(new LibraryVariableSetResource
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
        public Task<LibraryVariableSetEditor> CreateOrModify(string name, string description)
            => CreateOrModify(name, description, CancellationToken.None);

        public async Task<LibraryVariableSetEditor> CreateOrModify(string name, string description, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(name, cancellationToken).ConfigureAwait(false);

            if (existing == null)
            {
                Instance = await repository.Create(new LibraryVariableSetResource
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

        public LibraryVariableSetEditor Customize(Action<LibraryVariableSetResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<LibraryVariableSetEditor> Save()
            => Save(CancellationToken.None);

        public async Task<LibraryVariableSetEditor> Save(CancellationToken cancellationToken)
        {
            Instance = await repository.Modify(Instance, cancellationToken).ConfigureAwait(false);
            if (variables.IsValueCreated)
            {
                var vars = await variables.Value.ConfigureAwait(false);
                await vars.Save(cancellationToken).ConfigureAwait(false);
            }
            return this;
        }
    }
}
