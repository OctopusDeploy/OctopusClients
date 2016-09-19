using System;
using System.Threading.Tasks;
using Octopus.Client.Model.Accounts;

namespace Octopus.Client.Repositories
{
    public interface IAccountRepository : ICreate<AccountResource>, IModify<AccountResource>, IDelete<AccountResource>, IGet<AccountResource>, IFindByName<AccountResource>
    {
    }
}