using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Editors.Async
{
    public class AzureServicePrincipalAccountEditor : AccountEditor<AzureServicePrincipalAccountResource, AzureServicePrincipalAccountEditor>
    {
        private readonly IOctopusAsyncClient client;

        public AzureServicePrincipalAccountEditor(
            IOctopusAsyncClient client,
            IAccountRepository repository) : base(repository)
        {
            this.client = client;
        }

        public Task<List<AzureServicePrincipalAccountResource.ResourceGroup>> ResourceGroups()
        {
            return client.Get<List<AzureServicePrincipalAccountResource.ResourceGroup>>(Instance.Link("ResourceGroups"));
        }

        public Task<List<AzureServicePrincipalAccountResource.WebSite>> WebSites()
        {
            return client.Get<List<AzureServicePrincipalAccountResource.WebSite>>(Instance.Link("WebSites"));
        }

        public Task<List<AzureStorageAccount>> StorageAccounts()
        {
            return client.Get<List<AzureStorageAccount>>(Instance.Link("StorageAccounts"));
        }
    }
}