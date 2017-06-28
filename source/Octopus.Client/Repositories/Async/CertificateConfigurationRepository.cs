using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ICertificateConfigurationRepository : IGet<CertificateConfigurationResource>, IFindByName<CertificateConfigurationResource>
    {
        Task<CertificateConfigurationResource> GetOctopusCertificate();
    }

    class CertificateConfigurationRepository : BasicRepository<CertificateConfigurationResource>, ICertificateConfigurationRepository
    {
        public CertificateConfigurationRepository(IOctopusAsyncClient client) : base(client, DetermineCollectionLinkName(client))
        {
        }

        public Task<CertificateConfigurationResource> GetOctopusCertificate()
        {
            return Get("certificate-global");
        }

        static string DetermineCollectionLinkName(IOctopusAsyncClient client)
        {
            if (client.RootDocument == null)
                throw new NullReferenceException("The client root document is null");

            // For backwards compatibility. 
            // In Octopus 3.11, what was Certificates was moved to CertificatesConfiguration, to make room for the certificates feature.
            // This allows pre-3.11 clients to still work.
            return client.RootDocument.Links.ContainsKey("CertificateConfiguration")
                ? "CertificateConfiguration"
                : "Certificates";
        }
    }
}