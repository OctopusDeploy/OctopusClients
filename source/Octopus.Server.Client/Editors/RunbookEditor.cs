using System;
using Octopus.Client.Model;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class RunbookEditor : IResourceEditor<RunbookResource, RunbookEditor>
    {
        private readonly IRunbookRepository repository;
        private readonly Lazy<RunbookProcessEditor> runbookProcess;

        public RunbookEditor(IRunbookRepository repository,
            IRunbookProcessRepository runbookProcessRepository)
        {
            this.repository = repository;
            runbookProcess = new Lazy<RunbookProcessEditor>(() => new RunbookProcessEditor(runbookProcessRepository).Load(Instance.RunbookProcessId));
        }

        public RunbookResource Instance { get; private set; }

        public RunbookProcessEditor RunbookProcess => runbookProcess.Value;

        public RunbookEditor CreateOrModify(ProjectResource project, string name, string description)
        {
            var existing = repository.FindByName(project, name);

            if (existing == null)
            {
                Instance = repository.Create(new RunbookResource
                {
                    ProjectId = project.Id,
                    Name = name,
                    Description = description,
                    EnvironmentScope = RunbookEnvironmentScope.All,
                    DefaultGuidedFailureMode = GuidedFailureMode.EnvironmentDefault,
                    RunRetentionPolicy = new RunbookRetentionPeriod { QuantityToKeep = 100 }
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

        public RunbookEditor CreateOrModifyInGit(ProjectResource project, string slug, string name, string description, string gitRef, string commitMessage)
        {
            var existing = repository.GetInGit(slug, project, gitRef);
            
            if (existing == null)
            {
                Instance = repository.CreateInGit(new RunbookResource
                    {
                        Slug = slug,
                        ProjectId = project.Id,
                        Name = name,
                        Description = description
                    }, 
                    gitRef, 
                    commitMessage);
            }
            else
            {
                existing.Name = name;
                existing.Description = description;

                Instance = repository.ModifyInGit(existing, gitRef, commitMessage);
            }

            return this;
        }
        
        public RunbookEditor Load(string id)
        {
            Instance = repository.Get(id);
            return this;
        }
        
        public RunbookEditor LoadInGit(string id, ProjectResource project, string gitRef)
        { 
            Instance = repository.GetInGit(id, project, gitRef);
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
            if (runbookProcess.IsValueCreated)
            {
                runbookProcess.Value.Save();
            }
            return this;
        }
        
        public RunbookEditor SaveInGit(string gitRef, string commitMessage)
        {
            Instance = repository.ModifyInGit(Instance, gitRef,  commitMessage);

            if (runbookProcess.IsValueCreated)
            {
                var steps = runbookProcess.Value;
                steps.SaveInGit(gitRef, commitMessage);
            }
            
            return this;
        }
    }
}