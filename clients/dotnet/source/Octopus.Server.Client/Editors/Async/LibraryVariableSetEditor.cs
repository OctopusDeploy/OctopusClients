using System;
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
            variables = new Lazy<Task<VariableSetEditor>>(() => new VariableSetEditor(variableSetRepository).Load(Instance.VariableSetId));
        }

        public LibraryVariableSetResource Instance { get; private set; }

        public Task<VariableSetEditor> Variables => variables.Value;

        public IVariableTemplateContainerEditor<LibraryVariableSetResource> VariableTemplates => Instance;

        public async Task<LibraryVariableSetEditor> CreateOrModify(string name)
        {
            var existing = await repository.FindByName(name).ConfigureAwait(false);

            if (existing == null)
            {
                Instance = await repository.Create(new LibraryVariableSetResource
                {
                    Name = name,
                }).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;

                Instance = await repository.Modify(existing).ConfigureAwait(false);
            }

            return this;
        }

        public async Task<LibraryVariableSetEditor> CreateOrModify(string name, string description)
        {
            var existing = await repository.FindByName(name).ConfigureAwait(false);

            if (existing == null)
            {
                Instance = await repository.Create(new LibraryVariableSetResource
                {
                    Name = name,
                    Description = description
                }).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Description = description;

                Instance = await repository.Modify(existing).ConfigureAwait(false);
            }

            return this;
        }

        public LibraryVariableSetEditor Customize(Action<LibraryVariableSetResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<LibraryVariableSetEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            if (variables.IsValueCreated)
            {
                var vars = await variables.Value.ConfigureAwait(false);
                await vars.Save().ConfigureAwait(false);
            }
            return this;
        }
    }
}