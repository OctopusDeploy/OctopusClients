using System.Collections.Generic;
using System.Threading.Tasks;
using Octopus.Client.Model.Accounts;

namespace Octopus.Client.Repositories.Async
{
    public interface IAzureServicePrincipalAccountRepository
    {
        Task<List<AzureServicePrincipalAccountResource.WebSiteResource>> WebSites(AzureServicePrincipalAccountResource account);
    }

    public class AzureServicePrincipalAccountRepository : IAzureServicePrincipalAccountRepository
    {
        private readonly IOctopusAsyncClient client;

        public AzureServicePrincipalAccountRepository(IOctopusAsyncClient client)
        {
            this.client = client;
        }

        public Task<List<AzureServicePrincipalAccountResource.WebSiteResource>> WebSites(AzureServicePrincipalAccountResource account)
        {
            return client.Get<List<AzureServicePrincipalAccountResource.WebSiteResource>>(account.Link("WebSites"));
        }
    }
}