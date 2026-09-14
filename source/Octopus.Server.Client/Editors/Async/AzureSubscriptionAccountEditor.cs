using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class AzureSubscriptionAccountEditor : AccountEditor<AzureSubscriptionAccountResource, AzureSubscriptionAccountEditor>
    {
        public AzureSubscriptionAccountEditor(IAccountRepository repository) : base(repository)
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<AzureStorageAccount>> StorageAccounts(AzureSubscriptionAccountResource account)
            => StorageAccounts(account, CancellationToken.None);

        public Task<List<AzureStorageAccount>> StorageAccounts(AzureSubscriptionAccountResource account, CancellationToken cancellationToken)
        {
            return Repository.Client.Get<List<AzureStorageAccount>>(account.Link("StorageAccounts"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<AzureSubscriptionAccountResource.WebSite>> WebSites(AzureSubscriptionAccountResource account)
            => WebSites(account, CancellationToken.None);

        public Task<List<AzureSubscriptionAccountResource.WebSite>> WebSites(AzureSubscriptionAccountResource account, CancellationToken cancellationToken)
        {
            return Repository.Client.Get<List<AzureSubscriptionAccountResource.WebSite>>(account.Link("WebSites"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<AzureSubscriptionAccountResource.WebSite>> WebSites()
            => WebSites(CancellationToken.None);

        public Task<List<AzureSubscriptionAccountResource.WebSite>> WebSites(CancellationToken cancellationToken)
        {
            return Repository.Client.Get<List<AzureSubscriptionAccountResource.WebSite>>(Instance.Link("WebSites"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<AzureSubscriptionAccountResource.WebSlot>> WebSlots(AzureSubscriptionAccountResource account,
            AzureSubscriptionAccountResource.WebSite site)
            => WebSlots(account, site, CancellationToken.None);

        public Task<List<AzureSubscriptionAccountResource.WebSlot>> WebSlots(AzureSubscriptionAccountResource account,
            AzureSubscriptionAccountResource.WebSite site, CancellationToken cancellationToken)
        {
            return Repository.Client.Get<List<AzureSubscriptionAccountResource.WebSlot>>(account.Link("WebSlots"),
                new { resourceGroupName = site.ResourceGroup, webSiteName = site.WebSpace }, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<AzureSubscriptionAccountResource.WebSlot>> WebSlots(AzureSubscriptionAccountResource.WebSite site)
            => WebSlots(site, CancellationToken.None);

        public Task<List<AzureSubscriptionAccountResource.WebSlot>> WebSlots(AzureSubscriptionAccountResource.WebSite site, CancellationToken cancellationToken)
        {
            return Repository.Client.Get<List<AzureSubscriptionAccountResource.WebSlot>>(Instance.Link("WebSlots"),
                new { resourceGroupName = site.ResourceGroup, webSiteName = site.WebSpace }, cancellationToken);
        }
    }
}
