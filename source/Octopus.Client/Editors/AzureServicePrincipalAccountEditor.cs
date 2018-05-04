using System;
using System.Collections.Generic;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class AzureServicePrincipalAccountEditor : AccountEditor<AzureServicePrincipalAccountResource>
    {
        private readonly IOctopusClient client;

        public AzureServicePrincipalAccountEditor(
            IOctopusClient client,
            IAccountRepository repository) : base(repository)
        {
            this.client = client;
        }

        public List<AzureServicePrincipalAccountResource.ResourceGroup> ResourceGroups()
        {
            return client.Get<List<AzureServicePrincipalAccountResource.ResourceGroup>>(Instance.Link("ResourceGroups"));
        }

        public List<AzureServicePrincipalAccountResource.WebSite> WebSites()
        {
            return client.Get<List<AzureServicePrincipalAccountResource.WebSite>>(Instance.Link("WebSites"));
        }

        public List<AzureStorageAccount> StorageAccounts()
        {
            return client.Get<List<AzureStorageAccount>>(Instance.Link("StorageAccounts"));
        }
    }
}