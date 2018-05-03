using Octopus.Client.Model.Accounts;

namespace Octopus.Client.Repositories.Async
{
    public interface IAzureServicePrincipalAccountRepository
    {
        void WebSites(AzureServicePrincipalAccountResource account);
    }

    public class AzureServicePrincipalAccountRepository : IAzureServicePrincipalAccountRepository
    {
        private readonly IOctopusAsyncClient client;

        public AzureServicePrincipalAccountRepository(IOctopusAsyncClient client)
        {
            this.client = client;
        }

        public void WebSites(AzureServicePrincipalAccountResource account)
        {
            client.Get<string>(account.Link("WebSites"));
        }
    }
}