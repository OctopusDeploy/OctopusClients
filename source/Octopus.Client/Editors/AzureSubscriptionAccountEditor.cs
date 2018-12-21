using System.Collections.Generic;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class AzureSubscriptionAccountEditor : AccountEditor<AzureSubscriptionAccountResource, AzureSubscriptionAccountEditor>
    {
        public AzureSubscriptionAccountEditor(IAccountRepository repository) : base(repository)
        {
        }

        public List<AzureCloudService> CloudServices(AzureSubscriptionAccountResource account)
        {
            return Repository.Client.Get<List<AzureCloudService>>(account.Link("CloudServices"));
        }

        public List<AzureStorageAccount> StorageAccounts(AzureSubscriptionAccountResource account)
        {
            return Repository.Client.Get<List<AzureStorageAccount>>(account.Link("StorageAccounts"));
        }

        public List<AzureSubscriptionAccountResource.WebSite> WebSites(AzureSubscriptionAccountResource account)
        {
            return Repository.Client.Get<List<AzureSubscriptionAccountResource.WebSite>>(account.Link("WebSites"));
        }

        public List<AzureSubscriptionAccountResource.WebSite> WebSites()
        {
            return Repository.Client.Get<List<AzureSubscriptionAccountResource.WebSite>>(Instance.Link("WebSites"));
        }

        public List<AzureSubscriptionAccountResource.WebSlot> WebSiteSlots(AzureSubscriptionAccountResource account, AzureSubscriptionAccountResource.WebSite site)
        {
            return Repository.Client.Get<List<AzureSubscriptionAccountResource.WebSlot>>(account.Link("WebSiteSlots"),
                new { resourceGroupName = site.ResourceGroup, webSiteName = site.WebSpace});
        }
        public List<AzureSubscriptionAccountResource.WebSlot> WebSiteSlots(AzureSubscriptionAccountResource.WebSite site)
        {
            return Repository.Client.Get<List<AzureSubscriptionAccountResource.WebSlot>>(Instance.Link("WebSiteSlots"),
                new { resourceGroupName = site.ResourceGroup, webSiteName = site.WebSpace });
        }
    }
}