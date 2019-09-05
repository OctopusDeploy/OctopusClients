using System;
using System.Threading.Tasks;
using Octopus.Client.Model.Processes;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class ProcessEditor : IResourceEditor<ProcessResource, ProcessEditor>
    {
        private readonly IProcessRepository repository;

        public ProcessEditor(IProcessRepository repository)
        {
            this.repository = repository;
        }

        public ProcessResource Instance { get; private set; }

        public async Task<ProcessEditor> Load(string id)
        {
            Instance = await repository.Get(id).ConfigureAwait(false);
            return this;
        }

        public ProcessEditor Customize(Action<ProcessResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<ProcessEditor> Save()
        {
            Instance = await repository.Modify(Instance).ConfigureAwait(false);
            return this;
        }
    }
}