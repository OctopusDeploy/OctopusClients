using System.Collections.Generic;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class AzureSubscriptionAccountEditor : AccountEditor<AzureSubscriptionAccountResource, AzureSubscriptionAccountEditor>
    {
        private readonly IOctopusClient client;

        public AzureSubscriptionAccountEditor(
            IOctopusClient client,
            IAccountRepository repository) : base(repository)
        {
            this.client = client;
        }

        public List<AzureCloudService> CloudServices(AzureSubscriptionAccountResource account)
        {
            return client.Get<List<AzureCloudService>>(account.Link("CloudServices"));
        }

        public List<AzureStorageAccount> StorageAccounts(AzureSubscriptionAccountResource account)
        {
            return client.Get<List<AzureStorageAccount>>(account.Link("StorageAccounts"));
        }

        public List<AzureSubscriptionAccountResource.WebSite> WebSites(AzureSubscriptionAccountResource account)
        {
            return client.Get<List<AzureSubscriptionAccountResource.WebSite>>(account.Link("WebSites"));
        }
    }
}