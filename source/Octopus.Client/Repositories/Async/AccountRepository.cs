using System;
using Octopus.Client.Model.Accounts;

namespace Octopus.Client.Repositories.Async
{
    public interface IAccountRepository : ICreate<AccountResource>, IModify<AccountResource>, IDelete<AccountResource>, IGet<AccountResource>, IFindByName<AccountResource>
    {
    }

    class AccountRepository : BasicRepository<AccountResource>, IAccountRepository
    {
        public AccountRepository(IOctopusAsyncClient client)
            : base(client, "Accounts")
        {
        }
    }
}
