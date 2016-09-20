using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class VariableSetEditor : IResourceEditor<VariableSetResource, VariableSetEditor>
    {
        private readonly IVariableSetRepository repository;

        public VariableSetEditor(IVariableSetRepository repository)
        {
            this.repository = repository;
        }

        public VariableSetResource Instance { get; private set; }

        public async Task<VariableSetEditor> Load(string id)
        {
            Instance = await repository.Get(id).ConfigureAwait(false);
            return this;
        }

        public VariableSetEditor AddOrUpdateVariableValue(string name, string value, ScopeSpecification scope, bool isSensitive)
        {
            Instance.AddOrUpdateVariableValue(name, value, scope, isSensitive);
            return this;
        }

        public VariableSetEditor AddOrUpdateVariableValue(string name, string value, ScopeSpecification scope)
        {
            Instance.AddOrUpdateVariableValue(name, value, scope);
            return this;
        }

        public VariableSetEditor AddOrUpdateVariableValue(string name, string value)
        {
            Instance.AddOrUpdateVariableValue(name, value);
            return this;
        }

        public VariableSetEditor Customize(Action<VariableSetResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<VariableSetEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}