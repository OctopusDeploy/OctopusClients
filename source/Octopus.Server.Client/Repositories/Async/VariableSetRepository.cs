using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IVariableSetRepository : IGet<VariableSetResource>, IModify<VariableSetResource>, IGetAll<VariableSetResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<string[]> GetVariableNames(string projects, string[] environments, string gitRef = null);
        Task<string[]> GetVariableNames(string projects, string[] environments, CancellationToken cancellationToken, string gitRef = null);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<VariableSetResource> GetVariablePreview(string project, string channel, string tenant, string runbook, string action, string environment, string machine, string role);
        Task<VariableSetResource> GetVariablePreview(string project, string channel, string tenant, string runbook, string action, string environment, string machine, string role, CancellationToken cancellationToken);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<VariableSetResource> GetVariablePreview(ProjectResource projectResource, string channel = null, string tenant = null, string runbook = null, string action = null, string environment = null, string machine = null, string role = null, string gitRef = null);
        Task<VariableSetResource> GetVariablePreview(ProjectResource projectResource, CancellationToken cancellationToken, string channel = null, string tenant = null, string runbook = null, string action = null, string environment = null, string machine = null, string role = null, string gitRef = null);
        
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<VariableSetResource> Get(ProjectResource projectResource, string gitRef = null);
        Task<VariableSetResource> Get(ProjectResource projectResource, CancellationToken cancellationToken, string gitRef = null);
    }

    class VariableSetRepository : BasicRepository<VariableSetResource>, IVariableSetRepository
    {
        public VariableSetRepository(IOctopusAsyncRepository repository)
            : base(repository, "Variables")
        {
        }

        public async Task<string[]> GetVariableNames(string project, string[] environments, string gitRef = null)
        {
            return await GetVariableNames(project, environments, CancellationToken.None, gitRef);
        }
        
        public async Task<string[]> GetVariableNames(string project, string[] environments, CancellationToken cancellationToken, string gitRef = null)
        {
            return await Client.Get<string[]>(await Repository.Link("VariableNames").ConfigureAwait(false), cancellationToken, new {project, projectEnvironmentsFilter = environments ?? Array.Empty<string>(), gitRef}).ConfigureAwait(false);
        }

        public async Task<VariableSetResource> Get(ProjectResource projectResource, string gitRef = null)
        {
            return await Get(projectResource, CancellationToken.None, gitRef);
        }
        
        public async Task<VariableSetResource> Get(ProjectResource projectResource, CancellationToken cancellationToken, string gitRef = null)
        {
            if (ProjectHasVariablesInGit(projectResource))
            {
                if (gitRef is not null)
                {
                    return await Client.Get<VariableSetResource>(projectResource.Link("Variables"), cancellationToken, new {gitRef}).ConfigureAwait(false);
                }

                return await Client.Get<VariableSetResource>(projectResource.Link("SensitiveVariables"), cancellationToken).ConfigureAwait(false);
            }

            return await Client.Get<VariableSetResource>(projectResource.Link("Variables"), cancellationToken).ConfigureAwait(false);
        }

        public async Task<VariableSetResource> GetVariablePreview(string project, string channel, string tenant, string runbook, string action, string environment, string machine, string role)
        {
            return await GetVariablePreview(project: project, channel: channel, tenant: tenant, runbook: runbook, action: action, environment: environment, machine: machine, role: role, CancellationToken.None);
        }
        
        public async Task<VariableSetResource> GetVariablePreview(string project, string channel, string tenant, string runbook, string action, string environment, string machine, string role, CancellationToken cancellationToken)
        {
            return await Client.Get<VariableSetResource>(await Repository.Link("VariablePreview").ConfigureAwait(false), cancellationToken, new {project, channel, tenant, runbook, action, environment, machine, role}).ConfigureAwait(false);
        }

        public async Task<VariableSetResource> GetVariablePreview(ProjectResource projectResource, string channel = null, string tenant = null, string runbook = null, string action = null, string environment = null, string machine = null, string role = null, string gitRef = null)
        {
            return await GetVariablePreview(projectResource, CancellationToken.None, channel: channel, tenant: tenant, runbook: runbook, action: action, environment: environment, machine: machine, role: role, gitRef: gitRef);
        }
        
        public async Task<VariableSetResource> GetVariablePreview(ProjectResource projectResource, CancellationToken cancellationToken, string channel = null, string tenant = null, string runbook = null, string action = null, string environment = null, string machine = null, string role = null, string gitRef = null)
        {
            var project = projectResource.Id;
            return await Client.Get<VariableSetResource>(await Repository.Link("VariablePreview"), cancellationToken, new {project, channel, tenant, runbook, action, environment, machine, role, gitRef});
        }

        public override Task<List<VariableSetResource>> Get(params string[] ids)
        {
            throw new NotSupportedException("VariableSet does not support this operation");
        }

        bool ProjectHasVariablesInGit(ProjectResource projectResource)
        {
            return projectResource.PersistenceSettings is GitPersistenceSettingsResource gitSettings && gitSettings.ConversionState.VariablesAreInGit;
        }
    }
}