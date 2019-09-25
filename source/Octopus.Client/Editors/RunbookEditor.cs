using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class RunbookEditor : IResourceEditor<RunbookResource, RunbookEditor>
    {
        private readonly IRunbookRepository repository;

        public RunbookEditor(IRunbookRepository repository)
        {
            this.repository = repository;
        }

        public RunbookResource Instance { get; private set; }

        public RunbookEditor Load(string id)
        {
            Instance = repository.Get(id);
            return this;
        }

        public RunbookEditor Customize(Action<RunbookResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public RunbookEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}