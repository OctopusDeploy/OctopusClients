using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class RunbookProcessEditor : IResourceEditor<RunbookProcessResource, RunbookProcessEditor>
    {
        private readonly IRunbookProcessRepository repository;

        public RunbookProcessEditor(IRunbookProcessRepository repository)
        {
            this.repository = repository;
        }

        public RunbookProcessResource Instance { get; private set; }

        public RunbookProcessEditor Load(string id)
        {
            Instance = repository.Get(id);
            return this;
        }

        public RunbookProcessEditor LoadInGit(ProjectResource project, string id, string gitRef)
        {
            Instance = repository.GetInGit(project, id, gitRef);
            return this;
        }
        
        public DeploymentStepResource FindStep(string name)
        {
            return Instance.FindStep(name);
        }

        public DeploymentStepResource AddOrUpdateStep(string name)
        {
            return Instance.AddOrUpdateStep(name);
        }

        public RunbookProcessEditor RemoveStep(string name)
        {
            Instance.RemoveStep(name);
            return this;
        }

        public RunbookProcessEditor ClearSteps()
        {
            Instance.ClearSteps();
            return this;
        }

        public RunbookProcessEditor Customize(Action<RunbookProcessResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public RunbookProcessEditor Save()
        {
            Instance = repository.Modify(Instance);
            return this;
        }
        
        public RunbookProcessEditor SaveInGit(string gitRef, string commitMessage)
        {
            Instance = repository.ModifyInGit(gitRef, commitMessage, Instance);
            return this;
        }
    }
}