using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IVariableSetRepository : IGet<VariableSetResource>, IModify<VariableSetResource>
    {
        string[] GetVariableNames(string projects, string[] environments);
    }
    
    class VariableSetRepository : BasicRepository<VariableSetResource>, IVariableSetRepository
    {
        public VariableSetRepository(IOctopusClient client)
            : base(client, "Variables")
        {
        }

        public string[] GetVariableNames(string project, string[] environments)
        {
            return Client.Get<string[]>(Client.RootDocument.Link("VariableNames"), new { project, projectEnvironmentsFilter = environments ?? new string[0] });
        }

    }
}