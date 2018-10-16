using System;
using System.Collections.Generic;
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

        public Task<List<AzureServicePrincipalAccountResource.ResourceGroup>> ResourceGroups()
        {
            return Repository.Client.Get<List<AzureServicePrincipalAccountResource.ResourceGroup>>(Instance.Link("ResourceGroups"));
        }

        public Task<List<AzureServicePrincipalAccountResource.WebSite>> WebSites()
        {
            return Repository.Client.Get<List<AzureServicePrincipalAccountResource.WebSite>>(Instance.Link("WebSites"));
        }

        public Task<List<AzureServicePrincipalAccountResource.WebSlot>> WebSlots(AzureServicePrincipalAccountResource.WebSite site)
        {
            return Repository.Client.Get<List<AzureServicePrincipalAccountResource.WebSlot>>(Instance.Link("WebSlots"),
                new {id = Instance.Id, resourceGroupName = site.ResourceGroup, webSiteName = site.WebSpace});
        }

        public Task<List<AzureStorageAccount>> StorageAccounts()
        {
            return Repository.Client.Get<List<AzureStorageAccount>>(Instance.Link("StorageAccounts"));
        }
    }
}