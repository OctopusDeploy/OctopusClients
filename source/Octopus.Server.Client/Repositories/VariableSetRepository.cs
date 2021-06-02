using System;
using System.Collections.Generic;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IVariableSetRepository : IGet<VariableSetResource>, IModify<VariableSetResource>, IGetAll<VariableSetResource>
    {
        string[] GetVariableNames(string projects, string[] environments);
        VariableSetResource GetVariablePreview(string project, string channel, string tenant, string runbook, string action, string environment, string machine, string role);
    }
    
    class VariableSetRepository : BasicRepository<VariableSetResource>, IVariableSetRepository
    {
        public VariableSetRepository(IOctopusRepository repository)
            : base(repository, "Variables")
        {
        }

        public string[] GetVariableNames(string project, string[] environments)
        {
            return Client.Get<string[]>(Repository.Link("VariableNames"), new { project, projectEnvironmentsFilter = environments ?? new string[0] });
        }

        public VariableSetResource GetVariablePreview(string project, string channel, string tenant, string runbook, string action, string environment, string machine, string role)
        {
            return Client.Get<VariableSetResource>(Repository.Link("VariablePreview"), new { project, channel, tenant, runbook, action, environment, machine, role });
        }

        public override List<VariableSetResource> Get(params string[] ids)
        {
            throw new NotSupportedException("VariableSet does not support this operation");
        }
    }
}