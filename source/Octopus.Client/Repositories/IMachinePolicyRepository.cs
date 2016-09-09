using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface IMachinePolicyRepository : IFindByName<MachinePolicyResource>, ICreate<MachinePolicyResource>, IModify<MachinePolicyResource>, IGet<MachinePolicyResource>, IDelete<MachinePolicyResource>
    {
    }
}