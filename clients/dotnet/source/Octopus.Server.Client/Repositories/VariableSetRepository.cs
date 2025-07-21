using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IVariableSetRepository : IGet<VariableSetResource>, IModify<VariableSetResource>, IGetAll<VariableSetResource>
    {
        string[] GetVariableNames(string projects, string[] environments, string gitRef = null);
        VariableSetResource GetVariablePreview(string project, string channel, string tenant, string runbook, string action, string environment, string machine, string role);
        VariableSetResource GetVariablePreview(ProjectResource projectResource, string channel = null, string tenant = null, string runbook = null, string action = null, string environment = null, string machine = null, string role = null, string gitRef = null);
        VariableSetResource Get(ProjectResource projectResource, string gitRef = null);
    }

    class VariableSetRepository : BasicRepository<VariableSetResource>, IVariableSetRepository
    {
        public VariableSetRepository(IOctopusRepository repository)
            : base(repository, "Variables")
        {
        }

        public string[] GetVariableNames(string project, string[] environments, string gitRef = null)
        {
            return Client.Get<string[]>(Repository.Link("VariableNames"), new {project, projectEnvironmentsFilter = environments ?? new string[0], gitRef});
        }

        public VariableSetResource Get(ProjectResource projectResource, string gitRef = null)
        {
            if (ProjectHasVariablesInGit(projectResource))
            {
                if (gitRef is not null)
                {
                    return Client.Get<VariableSetResource>(projectResource.Link("Variables"), new {gitRef});
                }

                return Client.Get<VariableSetResource>(projectResource.Link("SensitiveVariables"));
            }

            return Client.Get<VariableSetResource>(projectResource.Link("Variables"));
        }

        public VariableSetResource GetVariablePreview(string project, string channel, string tenant, string runbook, string action, string environment, string machine, string role)
        {
            return Client.Get<VariableSetResource>(Repository.Link("VariablePreview"), new {project, channel, tenant, runbook, action, environment, machine, role});
        }

        public VariableSetResource GetVariablePreview(ProjectResource projectResource, string channel = null, string tenant = null, string runbook = null, string action = null, string environment = null, string machine = null, string role = null, string gitRef = null)
        {
            var project = projectResource.Id;
            return Client.Get<VariableSetResource>(Repository.Link("VariablePreview"), new {project, channel, tenant, runbook, action, environment, machine, role, gitRef});
        }

        public override List<VariableSetResource> Get(params string[] ids)
        {
            throw new NotSupportedException("VariableSet does not support this operation");
        }

        bool ProjectHasVariablesInGit(ProjectResource projectResource)
        {
            return projectResource.PersistenceSettings is GitPersistenceSettingsResource gitSettings && gitSettings.ConversionState.VariablesAreInGit;
        }
    }
}