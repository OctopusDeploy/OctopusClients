using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class AzureSubscriptionAccountEditor : AccountEditor<AzureSubscriptionAccountResource, AzureSubscriptionAccountEditor>
    {
        private readonly IOctopusAsyncClient client;

        public AzureSubscriptionAccountEditor(
            IOctopusAsyncClient client,
            IAccountRepository repository) : base(repository)
        {
            this.client = client;
        }

        public Task<List<AzureCloudService>> CloudServices(AzureSubscriptionAccountResource account)
        {
            return client.Get<List<AzureCloudService>>(account.Link("CloudServices"));
        }

        public Task<List<AzureStorageAccount>> StorageAccounts(AzureSubscriptionAccountResource account)
        {
            return client.Get<List<AzureStorageAccount>>(account.Link("StorageAccounts"));
        }

        public Task<List<AzureSubscriptionAccountResource.WebSite>> WebSites(AzureSubscriptionAccountResource account)
        {
            return client.Get<List<AzureSubscriptionAccountResource.WebSite>>(account.Link("WebSites"));
        }
    }
}