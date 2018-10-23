using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IVariableSetRepository : IGet<VariableSetResource>, IModify<VariableSetResource>
    {
        string[] GetVariableNames(string projects, string[] environments);
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

        public override List<VariableSetResource> Get(params string[] ids)
        {
            throw new NotSupportedException("VariableSet does not support this operation");
        }
    }
}