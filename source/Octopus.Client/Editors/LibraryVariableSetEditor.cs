using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class LibraryVariableSetEditor : IResourceEditor<LibraryVariableSetResource, LibraryVariableSetEditor>
    {
        private readonly ILibraryVariableSetRepository repository;
        private readonly Lazy<VariableSetEditor> variables;

        public LibraryVariableSetEditor(ILibraryVariableSetRepository repository, IVariableSetRepository variableSetRepository)
        {
            this.repository = repository;
            variables = new Lazy<VariableSetEditor>(() => new VariableSetEditor(variableSetRepository).Load(Instance.VariableSetId));
        }

        public LibraryVariableSetResource Instance { get; private set; }

        public VariableSetEditor Variables => variables.Value;

        public IVariableTemplateContainerEditor<LibraryVariableSetResource> VariableTemplates => Instance;

        public LibraryVariableSetEditor CreateOrModify(string name)
        {
            var existing = repository.FindByName(name);

            if (existing == null)
            {
                Instance = repository.Create(new LibraryVariableSetResource
                {
                    Name = name,
                });
            }
            else
            {
                existing.Name = name;

                Instance = repository.Modify(existing);
            }

            return this;
        }

        public LibraryVariableSetEditor CreateOrModify(string name, string description)
        {
            var existing = repository.FindByName(name);

            if (existing == null)
            {
                Instance = repository.Create(new LibraryVariableSetResource
                {
                    Name = name,
                    Description = description
                });
            }
            else
            {
                existing.Name = name;
                existing.Description = description;

                Instance = repository.Modify(existing);
            }

            return this;
        }

        public LibraryVariableSetEditor Customize(Action<LibraryVariableSetResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public LibraryVariableSetEditor Save()
        {
            Instance = repository.Modify(Instance);
            if (variables.IsValueCreated)
            {
                variables.Value.Save();
            }
            return this;
        }
        
        public LibraryVariableSetUsageResource Usages()
        {
            return repository.Client.Get<LibraryVariableSetUsageResource>(Instance.Link("Usages"));
        }

    }
}