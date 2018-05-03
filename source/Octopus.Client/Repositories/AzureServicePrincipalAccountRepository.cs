using System.Collections.Generic;
using Octopus.Client.Model.Accounts;

namespace Octopus.Client.Repositories
{
    public interface IAzureServicePrincipalAccountRepository
    {
        List<AzureServicePrincipalAccountResource.WebSiteResource> WebSites(AzureServicePrincipalAccountResource account);
    }

    public class AzureServicePrincipalAccountRepository : IAzureServicePrincipalAccountRepository
    {
        private readonly IOctopusClient client;

        public AzureServicePrincipalAccountRepository(IOctopusClient client)
        {
            this.client = client;
        }

        public List<AzureServicePrincipalAccountResource.WebSiteResource> WebSites(AzureServicePrincipalAccountResource account)
        {
            return client.Get<List<AzureServicePrincipalAccountResource.WebSiteResource>>(account.Link("WebSites"));
        }
    }
}