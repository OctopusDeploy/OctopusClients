using System;
using Octopus.Client.Model.Processes;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class ProcessEditor : IResourceEditor<ProcessResource, ProcessEditor>
    {
        private readonly IProcessRepository repository;

        public ProcessEditor(IProcessRepository repository)
        {
            this.repository = repository;
        }

        public ProcessResource Instance { get; private set; }

        public ProcessEditor Load(string id)
        {
            Instance = repository.Get(id);
            return this;
        }

        public ProcessEditor Customize(Action<ProcessResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public ProcessEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
    }
}