using System;
using System.Collections.Generic;
using Octopus.Client.Model.Accounts;
using Octopus.Client.Repositories;

namespace Octopus.Client.Editors
{
    public class AzureServicePrincipalAccountEditor : AccountEditor<AzureServicePrincipalAccountResource, AzureServicePrincipalAccountEditor>
    {
        public AzureServicePrincipalAccountEditor(IAccountRepository repository) : base(repository)
        {
        }

        public List<AzureServicePrincipalAccountResource.ResourceGroup> ResourceGroups()
        {
            return Repository.Client.Get<List<AzureServicePrincipalAccountResource.ResourceGroup>>(Instance.Link("ResourceGroups"));
        }

        public List<AzureServicePrincipalAccountResource.WebSite> WebSites()
        {
            return Repository.Client.Get<List<AzureServicePrincipalAccountResource.WebSite>>(Instance.Link("WebSites"));
        }
        public List<AzureServicePrincipalAccountResource.WebSlot> WebSiteSlots(AzureServicePrincipalAccountResource.WebSite site)
        {
            return Repository.Client.Get<List<AzureServicePrincipalAccountResource.WebSlot>>(Instance.Link("WebSiteSlots"),
                new {resourceGroupName = site.ResourceGroup, webSiteName = site.Name});
        }

        public List<AzureStorageAccount> StorageAccounts()
        {
            return Repository.Client.Get<List<AzureStorageAccount>>(Instance.Link("StorageAccounts"));
        }
    }
}