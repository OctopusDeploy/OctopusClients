using System.Collections.Generic;
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

        public Task<List<AzureCloudService>> CloudServices(AzureSubscriptionAccountResource account)
        {
            return Repository.Client.Get<List<AzureCloudService>>(account.Link("CloudServices"));
        }

        public Task<List<AzureStorageAccount>> StorageAccounts(AzureSubscriptionAccountResource account)
        {
            return Repository.Client.Get<List<AzureStorageAccount>>(account.Link("StorageAccounts"));
        }

        public Task<List<AzureSubscriptionAccountResource.WebSite>> WebSites(AzureSubscriptionAccountResource account)
        {
            return Repository.Client.Get<List<AzureSubscriptionAccountResource.WebSite>>(account.Link("WebSites"));
        }

        public Task<List<AzureSubscriptionAccountResource.WebSite>> WebSites()
        {
            return Repository.Client.Get<List<AzureSubscriptionAccountResource.WebSite>>(Instance.Link("WebSites"));
        }

        public Task<List<AzureSubscriptionAccountResource.WebSlot>> WebSlots(AzureSubscriptionAccountResource account,
            AzureSubscriptionAccountResource.WebSite site)
        {
            return Repository.Client.Get<List<AzureSubscriptionAccountResource.WebSlot>>(account.Link("WebSlots"),
                new { resourceGroupName = site.ResourceGroup, webSiteName = site.WebSpace});
        }

        public Task<List<AzureSubscriptionAccountResource.WebSlot>> WebSlots(AzureSubscriptionAccountResource.WebSite site)
        {
            return Repository.Client.Get<List<AzureSubscriptionAccountResource.WebSlot>>(Instance.Link("WebSlots"),
                new { resourceGroupName = site.ResourceGroup, webSiteName = site.WebSpace });
        }
    }
}