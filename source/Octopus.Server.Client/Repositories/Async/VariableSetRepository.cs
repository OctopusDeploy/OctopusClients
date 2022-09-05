using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IVariableSetRepository : IGet<VariableSetResource>, IModify<VariableSetResource>, IGetAll<VariableSetResource>
    {
        Task<string[]> GetVariableNames(string projects, string[] environments, string gitRef = null);
        Task<VariableSetResource> GetVariablePreview(string project, string channel, string tenant, string runbook, string action, string environment, string machine, string role);
        Task<VariableSetResource> GetVariablePreview(ProjectResource projectResource, string channel = null, string tenant = null, string runbook = null, string action = null, string environment = null, string machine = null, string role = null, string gitRef = null);
        Task<VariableSetResource> Get(ProjectResource projectResource, string gitRef = null);
    }

    class VariableSetRepository : BasicRepository<VariableSetResource>, IVariableSetRepository
    {
        public VariableSetRepository(IOctopusAsyncRepository repository)
            : base(repository, "Variables")
        {
        }

        public async Task<string[]> GetVariableNames(string project, string[] environments, string gitRef = null)
        {
            return await Client.Get<string[]>(await Repository.Link("VariableNames").ConfigureAwait(false), new {project, projectEnvironmentsFilter = environments ?? new string[0], gitRef}).ConfigureAwait(false);
        }

        public async Task<VariableSetResource> Get(ProjectResource projectResource, string gitRef = null)
        {
            if (ProjectHasVariablesInGit(projectResource))
            {
                if (gitRef is not null)
                {
                    return await Client.Get<VariableSetResource>(projectResource.Link("Variables"), new {gitRef}).ConfigureAwait(false);
                }

                return await Client.Get<VariableSetResource>(projectResource.Link("SensitiveVariables")).ConfigureAwait(false);
            }

            return await Client.Get<VariableSetResource>(projectResource.Link("Variables")).ConfigureAwait(false);
        }

        public async Task<VariableSetResource> GetVariablePreview(string project, string channel, string tenant, string runbook, string action, string environment, string machine, string role)
        {
            return await Client.Get<VariableSetResource>(await Repository.Link("VariablePreview").ConfigureAwait(false), new {project, channel, tenant, runbook, action, environment, machine, role}).ConfigureAwait(false);
        }

        public async Task<VariableSetResource> GetVariablePreview(ProjectResource projectResource, string channel = null, string tenant = null, string runbook = null, string action = null, string environment = null, string machine = null, string role = null, string gitRef = null)
        {
            var project = projectResource.Id;
            return await Client.Get<VariableSetResource>(await Repository.Link("VariablePreview"), new {project, channel, tenant, runbook, action, environment, machine, role, gitRef});
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