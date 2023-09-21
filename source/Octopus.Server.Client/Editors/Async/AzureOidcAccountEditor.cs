using System.Collections.Generic;
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

        public Task<List<AzureOidcAccountResource.ResourceGroup>> ResourceGroups()
        {
            return Repository.Client.Get<List<AzureOidcAccountResource.ResourceGroup>>(Instance.Link("ResourceGroups"));
        }

        public Task<List<AzureOidcAccountResource.WebSite>> WebSites()
        {
            return Repository.Client.Get<List<AzureOidcAccountResource.WebSite>>(Instance.Link("WebSites"));
        }

        public Task<List<AzureOidcAccountResource.WebSlot>> WebSlots(AzureOidcAccountResource.WebSite site)
        {
            return Repository.Client.Get<List<AzureOidcAccountResource.WebSlot>>(Instance.Link("WebSlots"),
                new {id = Instance.Id, resourceGroupName = site.ResourceGroup, webSiteName = site.WebSpace});
        }

        public Task<List<AzureStorageAccount>> StorageAccounts()
        {
            return Repository.Client.Get<List<AzureStorageAccount>>(Instance.Link("StorageAccounts"));
        }
    }
}