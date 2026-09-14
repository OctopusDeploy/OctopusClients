using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class AzureOidcAccountEditor : AccountEditor<AzureSubscriptionAccountResource, AzureOidcAccountEditor>
    {
        public AzureOidcAccountEditor(IAccountRepository repository) : base(repository)
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<AzureOidcAccountResource.ResourceGroup>> ResourceGroups()
            => ResourceGroups(CancellationToken.None);

        public Task<List<AzureOidcAccountResource.ResourceGroup>> ResourceGroups(CancellationToken cancellationToken)
        {
            return Repository.Client.Get<List<AzureOidcAccountResource.ResourceGroup>>(Instance.Link("ResourceGroups"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<AzureOidcAccountResource.WebSite>> WebSites()
            => WebSites(CancellationToken.None);

        public Task<List<AzureOidcAccountResource.WebSite>> WebSites(CancellationToken cancellationToken)
        {
            return Repository.Client.Get<List<AzureOidcAccountResource.WebSite>>(Instance.Link("WebSites"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<AzureOidcAccountResource.WebSlot>> WebSlots(AzureOidcAccountResource.WebSite site)
            => WebSlots(site, CancellationToken.None);

        public Task<List<AzureOidcAccountResource.WebSlot>> WebSlots(AzureOidcAccountResource.WebSite site, CancellationToken cancellationToken)
        {
            return Repository.Client.Get<List<AzureOidcAccountResource.WebSlot>>(Instance.Link("WebSlots"),
                new { id = Instance.Id, resourceGroupName = site.ResourceGroup, webSiteName = site.WebSpace }, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<AzureStorageAccount>> StorageAccounts()
            => StorageAccounts(CancellationToken.None);

        public Task<List<AzureStorageAccount>> StorageAccounts(CancellationToken cancellationToken)
        {
            return Repository.Client.Get<List<AzureStorageAccount>>(Instance.Link("StorageAccounts"), cancellationToken);
        }
    }
}
