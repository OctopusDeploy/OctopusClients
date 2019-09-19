using System;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class OpsProcessEditor : IResourceEditor<OpsProcessResource, OpsProcessEditor>
    {
        private readonly IOpsProcessRepository repository;

        public OpsProcessEditor(IOpsProcessRepository repository)
        {
            this.repository = repository;
        }

        public OpsProcessResource Instance { get; private set; }

        public async Task<OpsProcessEditor> Load(string id)
        {
            Instance = await repository.Get(id).ConfigureAwait(false);
            return this;
        }

        public OpsProcessEditor Customize(Action<OpsProcessResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<OpsProcessEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}