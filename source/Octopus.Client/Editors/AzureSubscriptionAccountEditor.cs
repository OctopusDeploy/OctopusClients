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
    }
}