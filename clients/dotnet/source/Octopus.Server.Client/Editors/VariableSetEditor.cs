using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class VariableSetEditor : IResourceEditor<VariableSetResource, VariableSetEditor>
    {
        private readonly IVariableSetRepository repository;

        public VariableSetEditor(IVariableSetRepository repository)
        {
            this.repository = repository;
        }

        public VariableSetResource Instance { get; private set; }

        public VariableSetEditor Load(string id)
        {
            Instance = repository.Get(id);
            return this;
        }

        public VariableSetEditor AddOrUpdateVariableValue(string name, string value, ScopeSpecification scope, bool isSensitive)
        {
            Instance.AddOrUpdateVariableValue(name, value, scope, isSensitive);
            return this;
        }

        public VariableSetEditor AddOrUpdateVariableValue(string name, string value, ScopeSpecification scope, bool isSensitive, string description)
        {
            Instance.AddOrUpdateVariableValue(name, value, scope, isSensitive, description);
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

        public VariableSetEditor AddOrUpdateVariableValue(string name, string value, string description)
        {
            Instance.AddOrUpdateVariableValue(name, value, description);
            return this;
        }

        public VariableSetEditor Customize(Action<VariableSetResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public VariableSetEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}