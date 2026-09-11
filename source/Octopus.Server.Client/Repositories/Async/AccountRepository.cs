using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TAccount> GetOfType<TAccount>(string idOrHref) where TAccount : AccountResource;
        Task<TAccount> GetOfType<TAccount>(string idOrHref, CancellationToken cancellationToken) where TAccount : AccountResource;
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<TAccount>> GetOfType<TAccount>(params string[] ids) where TAccount : AccountResource;
        Task<List<TAccount>> GetOfType<TAccount>(CancellationToken cancellationToken, params string[] ids) where TAccount : AccountResource;
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TAccount> RefreshOfType<TAccount>(TAccount resource) where TAccount : AccountResource;
        Task<TAccount> RefreshOfType<TAccount>(TAccount resource, CancellationToken cancellationToken) where TAccount : AccountResource;

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TAccount> FindByNameOfType<TAccount>(string name) where TAccount : AccountResource;
        Task<TAccount> FindByNameOfType<TAccount>(string name, CancellationToken cancellationToken) where TAccount : AccountResource;
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<TAccount>> FindByNamesOfType<TAccount>(IEnumerable<string> names) where TAccount : AccountResource;
        Task<List<TAccount>> FindByNamesOfType<TAccount>(IEnumerable<string> names, CancellationToken cancellationToken) where TAccount : AccountResource;

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task PaginateOfType<TAccount>(Func<ResourceCollection<TAccount>, bool> getNextPage, object pathParameters = null) where TAccount : AccountResource;
        Task PaginateOfType<TAccount>(Func<ResourceCollection<TAccount>, bool> getNextPage, CancellationToken cancellationToken, object pathParameters = null) where TAccount : AccountResource;
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<TAccount> FindOneOfType<TAccount>(Func<TAccount, bool> search, object pathParameters = null) where TAccount : AccountResource;
        Task<TAccount> FindOneOfType<TAccount>(Func<TAccount, bool> search, CancellationToken cancellationToken, object pathParameters = null) where TAccount : AccountResource;
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<TAccount>> FindManyOfType<TAccount>(Func<TAccount, bool> search, object pathParameters = null) where TAccount : AccountResource;
        Task<List<TAccount>> FindManyOfType<TAccount>(Func<TAccount, bool> search, CancellationToken cancellationToken, object pathParameters = null) where TAccount : AccountResource;
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<List<TAccount>> FindAllOfType<TAccount>(object pathParameters = null) where TAccount : AccountResource;
        Task<List<TAccount>> FindAllOfType<TAccount>(CancellationToken cancellationToken, object pathParameters = null) where TAccount : AccountResource;

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<AccountUsageResource> GetAccountUsage(AccountResource account);
        Task<AccountUsageResource> GetAccountUsage(AccountResource account, CancellationToken cancellationToken);
    }

    class AccountRepository : BasicRepository<AccountResource>, IAccountRepository
    {
        public AccountRepository(IOctopusAsyncRepository repository)
            : base(repository, "Accounts")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TAccount> GetOfType<TAccount>(string idOrHref) where TAccount : AccountResource
            => GetOfType<TAccount>(idOrHref, CancellationToken.None);

        public async Task<TAccount> GetOfType<TAccount>(string idOrHref, CancellationToken cancellationToken) where TAccount : AccountResource
        {
            var account = await base.Get(idOrHref, cancellationToken).ConfigureAwait(false);
            return account as TAccount;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<TAccount>> GetOfType<TAccount>(params string[] ids) where TAccount : AccountResource
            => GetOfType<TAccount>(CancellationToken.None, ids);

        public async Task<List<TAccount>> GetOfType<TAccount>(CancellationToken cancellationToken, params string[] ids) where TAccount : AccountResource
        {
            var accounts = await base.Get(cancellationToken, ids).ConfigureAwait(false);
            return accounts as List<TAccount>;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TAccount> RefreshOfType<TAccount>(TAccount resource) where TAccount : AccountResource
            => RefreshOfType(resource, CancellationToken.None);

        public async Task<TAccount> RefreshOfType<TAccount>(TAccount resource, CancellationToken cancellationToken) where TAccount : AccountResource
        {
            var account = await base.Refresh(resource, cancellationToken).ConfigureAwait(false);
            return account as TAccount;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TAccount> FindByNameOfType<TAccount>(string name) where TAccount : AccountResource
            => FindByNameOfType<TAccount>(name, CancellationToken.None);

        public Task<TAccount> FindByNameOfType<TAccount>(string name, CancellationToken cancellationToken) where TAccount : AccountResource
        {
            var accountType = DetermineAccountType<TAccount>();
            name = (name ?? string.Empty).Trim();

            return FindOneOfType<TAccount>(r =>
            {
                if (r is INamedResource named)
                    return string.Equals((named.Name ?? string.Empty).Trim(), name, StringComparison.OrdinalIgnoreCase);
                return false;
            }, cancellationToken, pathParameters: new { accountType, name });
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<TAccount>> FindByNamesOfType<TAccount>(IEnumerable<string> names) where TAccount : AccountResource
            => FindByNamesOfType<TAccount>(names, CancellationToken.None);

        public Task<List<TAccount>> FindByNamesOfType<TAccount>(IEnumerable<string> names, CancellationToken cancellationToken) where TAccount : AccountResource
        {
            var nameSet = new HashSet<string>((names ?? new string[0]).Select(n => (n ?? string.Empty).Trim()), StringComparer.OrdinalIgnoreCase);
            return FindManyOfType<TAccount>(r =>
            {
                if (r is INamedResource named)
                    return nameSet.Contains((named.Name ?? string.Empty).Trim());
                return false;
            }, cancellationToken, pathParameters: DetermineAccountType<TAccount>());
        }

        object PathParametersOfType<TAccount>(object pathParameters) where TAccount : AccountResource
        {
            if (pathParameters != null)
                return pathParameters;
            var accountType = DetermineAccountType<TAccount>();
            return new { accountType };
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task PaginateOfType<TAccount>(Func<ResourceCollection<TAccount>, bool> getNextPage, object pathParameters = null) where TAccount : AccountResource
            => PaginateOfType<TAccount>(getNextPage, CancellationToken.None, pathParameters);

        public async Task PaginateOfType<TAccount>(Func<ResourceCollection<TAccount>, bool> getNextPage, CancellationToken cancellationToken, object pathParameters = null) where TAccount : AccountResource
        {
            await Client.Paginate(await Repository.Link(CollectionLinkName).ConfigureAwait(false), PathParametersOfType<TAccount>(pathParameters), getNextPage, cancellationToken).ConfigureAwait(false);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<TAccount> FindOneOfType<TAccount>(Func<TAccount, bool> search, object pathParameters = null) where TAccount : AccountResource
            => FindOneOfType<TAccount>(search, CancellationToken.None, pathParameters);

        public async Task<TAccount> FindOneOfType<TAccount>(Func<TAccount, bool> search, CancellationToken cancellationToken, object pathParameters = null) where TAccount : AccountResource
        {
            TAccount resource = null;
            await PaginateOfType<TAccount>(page =>
                {
                    resource = page.Items.FirstOrDefault(search);
                    return resource == null;
                }, cancellationToken, pathParameters: PathParametersOfType<TAccount>(pathParameters))
                .ConfigureAwait(false);
            return resource;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<TAccount>> FindManyOfType<TAccount>(Func<TAccount, bool> search, object pathParameters = null) where TAccount : AccountResource
            => FindManyOfType<TAccount>(search, CancellationToken.None, pathParameters);

        public async Task<List<TAccount>> FindManyOfType<TAccount>(Func<TAccount, bool> search, CancellationToken cancellationToken, object pathParameters = null) where TAccount : AccountResource
        {
            var resources = new List<TAccount>();
            await PaginateOfType<TAccount>(page =>
                {
                    resources.AddRange(page.Items.Where(search));
                    return true;
                }, cancellationToken, pathParameters: PathParametersOfType<TAccount>(pathParameters))
                .ConfigureAwait(false);
            return resources;
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<TAccount>> FindAllOfType<TAccount>(object pathParameters = null) where TAccount : AccountResource
            => FindAllOfType<TAccount>(CancellationToken.None, pathParameters);

        public Task<List<TAccount>> FindAllOfType<TAccount>(CancellationToken cancellationToken, object pathParameters = null) where TAccount : AccountResource
        {
            return FindManyOfType<TAccount>(x => true, cancellationToken, PathParametersOfType<TAccount>(pathParameters));
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<AccountUsageResource> GetAccountUsage(AccountResource account)
            => GetAccountUsage(account, CancellationToken.None);

        public async Task<AccountUsageResource> GetAccountUsage(AccountResource account, CancellationToken cancellationToken)
        {
            return await Client.Get<AccountUsageResource>(account.Link("Usages"), cancellationToken).ConfigureAwait(false);
        }

        public AccountType DetermineAccountType<TAccount>() where TAccount : AccountResource
        {
            return typeof(TAccount).DetermineAccountType();
        }
    }
}
