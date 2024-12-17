using System;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class RunbookEditor : IResourceEditor<RunbookResource, RunbookEditor>
    {
        private readonly IRunbookRepository repository;
        private readonly Lazy<Task<RunbookProcessEditor>> runbookProcess;

        public RunbookEditor(IRunbookRepository repository,
            IRunbookProcessRepository runbookProcessRepository)
        {
            this.repository = repository;
            runbookProcess = new Lazy<Task<RunbookProcessEditor>>(() => new RunbookProcessEditor(runbookProcessRepository).Load(Instance.RunbookProcessId));
        }

        public RunbookResource Instance { get; private set; }

        public Task<RunbookProcessEditor> RunbookProcess => runbookProcess.Value;

        
        public async Task<RunbookEditor> CreateOrModify(ProjectResource project, string name, string description)
            => await CreateOrModify(project, name, description, CancellationToken.None).ConfigureAwait(false);
        
        public async Task<RunbookEditor> CreateOrModify(ProjectResource project, string name, string description, CancellationToken cancellationToken)
        {
            var existing = await repository.FindByName(project, name).ConfigureAwait(false);
            
            if (existing == null)
            {
                Instance = await repository.Create(new RunbookResource
                {
                    ProjectId = project.Id,
                    Name = name,
                    Description = description
                }, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Description = description;

                Instance = await repository.Modify(existing, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }

        public async Task<RunbookEditor> CreateOrModifyInGit(ProjectResource project, string slug, string name, string description, string gitRef, string commitMessage, CancellationToken cancellationToken)
        {
            var existing = await repository.GetInGit(slug, project, gitRef, cancellationToken).ConfigureAwait(false);
            
            if (existing == null)
            {
                Instance = await repository.CreateInGit(new RunbookResource
                {
                    Slug = slug,
                    ProjectId = project.Id,
                    Name = name,
                    Description = description
                }, 
                    gitRef, 
                    commitMessage,
                    cancellationToken).ConfigureAwait(false);
            }
            else
            {
                existing.Name = name;
                existing.Description = description;

                Instance = await repository.ModifyInGit(existing, gitRef, commitMessage, cancellationToken).ConfigureAwait(false);
            }

            return this;
        }
        
        public async Task<RunbookEditor> Load(string id)
            => await Load(id, CancellationToken.None).ConfigureAwait(false);
        
        public async Task<RunbookEditor> Load(string id, CancellationToken cancellationToken)
        {
            Instance = await repository.Get(id, cancellationToken).ConfigureAwait(false);
            return this;
        }
        
        public async Task<RunbookEditor> LoadInGit(string id, ProjectResource project, string gitRef, CancellationToken cancellationToken)
        { 
            Instance = await repository.GetInGit(id, project, gitRef, cancellationToken).ConfigureAwait(false);
            return this;
        }

        public RunbookEditor Customize(Action<RunbookResource> customize)
        {
            customize?.Invoke(Instance);
            return this;
        }

        public async Task<RunbookEditor> Save()
        { 
            Instance = await repository.Modify(Instance).ConfigureAwait(false);

            if (runbookProcess.IsValueCreated)
            {
                var steps = await runbookProcess.Value.ConfigureAwait(false);
                await steps.Save().ConfigureAwait(false);
            }
            return this;
        }
        
        public async Task<RunbookEditor> SaveInGit(string gitRef, string commitMessage, CancellationToken cancellationToken)
        {
            Instance = await repository.ModifyInGit(Instance, gitRef,  commitMessage, cancellationToken).ConfigureAwait(false);

            if (runbookProcess.IsValueCreated)
            {
                var steps = await runbookProcess.Value.ConfigureAwait(false);
                await steps.SaveInGit(gitRef, commitMessage, cancellationToken).ConfigureAwait(false);
            }
            
            return this;
        }
    }
}