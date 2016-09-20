using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface IVariableSetRepository : IGet<VariableSetResource>, IModify<VariableSetResource>
    {
        Task<string[]> GetVariableNames(string projects, string[] environments);
    }

    class VariableSetRepository : BasicRepository<VariableSetResource>, IVariableSetRepository
    {
        public VariableSetRepository(IOctopusAsyncClient client)
            : base(client, "Variables")
        {
        }

        public Task<string[]> GetVariableNames(string project, string[] environments)
        {
            return Client.Get<string[]>(Client.RootDocument.Link("VariableNames"), new { project, projectEnvironmentsFilter = environments ?? new string[0] });
        }

    }
}
