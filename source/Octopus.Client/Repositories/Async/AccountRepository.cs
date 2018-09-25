using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensions;
using Octopus.Client.Model;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Model.Accounts.Usages;

namespace Octopus.Client.Repositories.Async
{
    public interface IAccountRepository : IResourceRepository, ICreate<AccountResource>, IModify<AccountResource>, IDelete<AccountResource>, IGet<AccountResource>, IFindByName<AccountResource>
    {
        AccountType DetermineAccountType<TAccount>() where TAccount : AccountResource;

        Task<TAccount> GetOfType<TAccount>(string idOrHref) where TAccount : AccountResource;
        Task<List<TAccount>> GetOfType<TAccount>(params string[] ids) where TAccount : AccountResource;
        Task<TAccount> RefreshOfType<TAccount>(TAccount resource) where TAccount : AccountResource;

        Task<TAccount> FindByNameOfType<TAccount>(string name) where TAccount : AccountResource;
        Task<List<TAccount>> FindByNamesOfType<TAccount>(IEnumerable<string> names) where TAccount : AccountResource;

        Task PaginateOfType<TAccount>(Func<ResourceCollection<TAccount>, bool> getNextPage, object pathParameters = null) where TAccount : AccountResource;
        Task<TAccount> FindOneOfType<TAccount>(Func<TAccount, bool> search, object pathParameters = null) where TAccount : AccountResource;
        Task<List<TAccount>> FindManyOfType<TAccount>(Func<TAccount, bool> search, object pathParameters = null) where TAccount : AccountResource;
        Task<List<TAccount>> FindAllOfType<TAccount>(object pathParameters = null) where TAccount : AccountResource;

        Task<AccountUsageResource> GetAccountUsage(AccountResource account);
    }

    class AccountRepository : BasicRepository<AccountResource>, IAccountRepository
    {
        public AccountRepository(IOctopusAsyncRepository repository)
            : base(repository, "Accounts")
        {
        }

        public async Task<TAccount> GetOfType<TAccount>(string idOrHref) where TAccount : AccountResource
        {
            var account = await base.Get(idOrHref);
            return account as TAccount;
        }

        public async Task<List<TAccount>> GetOfType<TAccount>(params string[] ids) where TAccount : AccountResource
        {
            var accounts = await base.Get(ids);
            return accounts as List<TAccount>;
        }

        public async Task<TAccount> RefreshOfType<TAccount>(TAccount resource) where TAccount : AccountResource
        {
            var account = await base.Refresh(resource);
            return account as TAccount;
        }

        public Task<TAccount> FindByNameOfType<TAccount>(string name) where TAccount : AccountResource
        {
            var accountType = DetermineAccountType<TAccount>();
            name = (name ?? string.Empty).Trim();

            return FindOneOfType<TAccount>(r =>
            {
                if (r is INamedResource named)
                    return string.Equals((named.Name ?? string.Empty).Trim(), name, StringComparison.OrdinalIgnoreCase);
                return false;
            }, pathParameters: new {accountType, name});
        }

        public Task<List<TAccount>> FindByNamesOfType<TAccount>(IEnumerable<string> names) where TAccount : AccountResource
        {
            var nameSet = new HashSet<string>((names ?? new string[0]).Select(n => (n ?? string.Empty).Trim()), StringComparer.OrdinalIgnoreCase);
            return FindManyOfType<TAccount>(r =>
            {
                if (r is INamedResource named) 
                    return nameSet.Contains((named.Name ?? string.Empty).Trim());
                return false;
            }, pathParameters: DetermineAccountType<TAccount>());
        }

        object PathParametersOfType<TAccount>(object pathParameters) where TAccount : AccountResource
        {
            if (pathParameters != null)
                return pathParameters;
            var accountType = DetermineAccountType<TAccount>();
            return new {accountType};
        }

        public Task PaginateOfType<TAccount>(Func<ResourceCollection<TAccount>, bool> getNextPage, object pathParameters = null) where TAccount : AccountResource
        {
            return Client.Paginate(Repository.Link(CollectionLinkName), PathParametersOfType<TAccount>(pathParameters), getNextPage);
        }

        public async Task<TAccount> FindOneOfType<TAccount>(Func<TAccount, bool> search, object pathParameters = null) where TAccount : AccountResource
        {
            TAccount resource = null;
            await PaginateOfType<TAccount>(page =>
                {
                    resource = page.Items.FirstOrDefault(search);
                    return resource == null;
                }, pathParameters: PathParametersOfType<TAccount>(pathParameters))
                .ConfigureAwait(false);
            return resource;
        }

        public async Task<List<TAccount>> FindManyOfType<TAccount>(Func<TAccount, bool> search, object pathParameters = null) where TAccount : AccountResource
        {
            var resources = new List<TAccount>();
            await PaginateOfType<TAccount>(page =>
                {
                    resources.AddRange(page.Items.Where(search));
                    return true;
                }, pathParameters: PathParametersOfType<TAccount>(pathParameters))
                .ConfigureAwait(false);
            return resources;
        }

        public Task<List<TAccount>> FindAllOfType<TAccount>(object pathParameters = null) where TAccount : AccountResource
        {
            return FindManyOfType<TAccount>(x => true, PathParametersOfType<TAccount>(pathParameters));
        }

        public async Task<AccountUsageResource> GetAccountUsage(AccountResource account)
        {
            return await Client.Get<AccountUsageResource>(account.Link("Usages"));
        }

        public AccountType DetermineAccountType<TAccount>() where TAccount : AccountResource
        {
            return typeof(TAccount).DetermineAccountType();
        }
    }
}
