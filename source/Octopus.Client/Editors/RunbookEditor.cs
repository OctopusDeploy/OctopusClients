using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class OpsProcessEditor : IResourceEditor<OpsProcessResource, OpsProcessEditor>
    {
        private readonly IOpsProcessRepository repository;

        public OpsProcessEditor(IOpsProcessRepository repository)
        {
            this.repository = repository;
        }

        public OpsProcessResource Instance { get; private set; }

        public OpsProcessEditor Load(string id)
        {
            Instance = repository.Get(id);
            return this;
        }

        public OpsProcessEditor Customize(Action<OpsProcessResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public OpsProcessEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}