using System;
using System.Collections.Generic;
using System.Linq;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensions;
using Octopus.Client.Model;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Model.Accounts.Usages;

namespace Octopus.Client.Repositories
{
    public interface IAccountRepository : IResourceRepository, ICreate<AccountResource>, IModify<AccountResource>, IDelete<AccountResource>, IGet<AccountResource>, IFindByName<AccountResource>
    {
        AccountType DetermineAccountType<TAccount>() where TAccount : AccountResource;

        TAccount GetOfType<TAccount>(string idOrHref) where TAccount : AccountResource;
        List<TAccount> GetOfType<TAccount>(params string[] ids) where TAccount : AccountResource;
        TAccount RefreshOfType<TAccount>(TAccount resource) where TAccount : AccountResource;

        TAccount FindByNameOfType<TAccount>(string name) where TAccount : AccountResource;
        List<TAccount> FindByNamesOfType<TAccount>(IEnumerable<string> names) where TAccount : AccountResource;

        void PaginateOfType<TAccount>(Func<ResourceCollection<TAccount>, bool> getNextPage, object pathParameters = null) where TAccount : AccountResource;
        TAccount FindOneOfType<TAccount>(Func<TAccount, bool> search, object pathParameters = null) where TAccount : AccountResource;
        List<TAccount> FindManyOfType<TAccount>(Func<TAccount, bool> search, object pathParameters = null) where TAccount : AccountResource;
        List<TAccount> FindAllOfType<TAccount>(object pathParameters = null) where TAccount : AccountResource;

        AccountUsageResource GetAccountUsage(AccountResource account);
    }
    
    class AccountRepository : BasicRepository<AccountResource>, IAccountRepository
    {
        public AccountRepository(IOctopusRepository repository)
            : base(repository, "Accounts")
        {
        }
        public TAccount GetOfType<TAccount>(string idOrHref) where TAccount : AccountResource
        {
            var account = base.Get(idOrHref);
            return account as TAccount;
        }

        public List<TAccount> GetOfType<TAccount>(params string[] ids) where TAccount : AccountResource
        {
            var accounts = base.Get(ids);
            return accounts as List<TAccount>;
        }

        public TAccount RefreshOfType<TAccount>(TAccount resource) where TAccount : AccountResource
        {
            var account = base.Refresh(resource);
            return account as TAccount;
        }

        public TAccount FindByNameOfType<TAccount>(string name) where TAccount : AccountResource
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

        public List<TAccount> FindByNamesOfType<TAccount>(IEnumerable<string> names) where TAccount : AccountResource
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

        public void PaginateOfType<TAccount>(Func<ResourceCollection<TAccount>, bool> getNextPage, object pathParameters = null) where TAccount : AccountResource
        {
            Client.Paginate(Repository.Link(CollectionLinkName), PathParametersOfType<TAccount>(pathParameters), getNextPage);
        }

        public TAccount FindOneOfType<TAccount>(Func<TAccount, bool> search, object pathParameters = null) where TAccount : AccountResource
        {
            TAccount resource = null;
            PaginateOfType<TAccount>(page =>
                {
                    resource = page.Items.FirstOrDefault(search);
                    return resource == null;
                }, pathParameters: PathParametersOfType<TAccount>(pathParameters));
            return resource;
        }

        public List<TAccount> FindManyOfType<TAccount>(Func<TAccount, bool> search, object pathParameters = null) where TAccount : AccountResource
        {
            var resources = new List<TAccount>();
            PaginateOfType<TAccount>(page =>
                {
                    resources.AddRange(page.Items.Where(search));
                    return true;
                }, pathParameters: PathParametersOfType<TAccount>(pathParameters));
            return resources;
        }

        public List<TAccount> FindAllOfType<TAccount>(object pathParameters = null) where TAccount : AccountResource
        {
            return FindManyOfType<TAccount>(x => true, PathParametersOfType<TAccount>(pathParameters));
        }

        public AccountType DetermineAccountType<TAccount>() where TAccount : AccountResource
        {
            return typeof(TAccount).DetermineAccountType();
        }

        public AccountUsageResource GetAccountUsage(AccountResource account)
        {
            return Client.Get<AccountUsageResource>(account.Link("Usages"));
        }
    }
}