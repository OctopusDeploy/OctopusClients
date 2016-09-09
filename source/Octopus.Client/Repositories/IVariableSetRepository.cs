using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IVariableSetRepository : IGet<VariableSetResource>, IModify<VariableSetResource>
    {
        string[] GetVariableNames(string projects, string[] environments);
    }
}