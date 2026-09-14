using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class AzureServicePrincipalAccountEditor : AccountEditor<AzureServicePrincipalAccountResource, AzureServicePrincipalAccountEditor>
    {
        public AzureServicePrincipalAccountEditor(IAccountRepository repository) : base(repository)
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<AzureServicePrincipalAccountResource.ResourceGroup>> ResourceGroups()
            => ResourceGroups(CancellationToken.None);

        public Task<List<AzureServicePrincipalAccountResource.ResourceGroup>> ResourceGroups(CancellationToken cancellationToken)
        {
            return Repository.Client.Get<List<AzureServicePrincipalAccountResource.ResourceGroup>>(Instance.Link("ResourceGroups"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<AzureServicePrincipalAccountResource.WebSite>> WebSites()
            => WebSites(CancellationToken.None);

        public Task<List<AzureServicePrincipalAccountResource.WebSite>> WebSites(CancellationToken cancellationToken)
        {
            return Repository.Client.Get<List<AzureServicePrincipalAccountResource.WebSite>>(Instance.Link("WebSites"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<List<AzureServicePrincipalAccountResource.WebSlot>> WebSlots(AzureServicePrincipalAccountResource.WebSite site)
            => WebSlots(site, CancellationToken.None);

        public Task<List<AzureServicePrincipalAccountResource.WebSlot>> WebSlots(AzureServicePrincipalAccountResource.WebSite site, CancellationToken cancellationToken)
        {
            return Repository.Client.Get<List<AzureServicePrincipalAccountResource.WebSlot>>(Instance.Link("WebSlots"),
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
