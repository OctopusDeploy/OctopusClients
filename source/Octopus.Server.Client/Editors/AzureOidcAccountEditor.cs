using System.Collections.Generic;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class AzureOidcAccountEditor : AccountEditor<AzureOidcAccountResource, AzureOidcAccountEditor>
    {
        public AzureOidcAccountEditor(IAccountRepository repository) : base(repository)
        {
        }

        public List<AzureOidcAccountResource.ResourceGroup> ResourceGroups()
        {
            return Repository.Client.Get<List<AzureOidcAccountResource.ResourceGroup>>(Instance.Link("ResourceGroups"));
        }

        public List<AzureOidcAccountResource.WebSite> WebSites()
        {
            return Repository.Client.Get<List<AzureOidcAccountResource.WebSite>>(Instance.Link("WebSites"));
        }

        public List<AzureOidcAccountResource.WebSlot> WebSiteSlots(AzureOidcAccountResource.WebSite site)
        {
            return Repository.Client.Get<List<AzureOidcAccountResource.WebSlot>>(Instance.Link("WebSiteSlots"),
                new {resourceGroupName = site.ResourceGroup, webSiteName = site.Name});
        }

        public List<AzureStorageAccount> StorageAccounts()
        {
            return Repository.Client.Get<List<AzureStorageAccount>>(Instance.Link("StorageAccounts"));
        }
    }
}