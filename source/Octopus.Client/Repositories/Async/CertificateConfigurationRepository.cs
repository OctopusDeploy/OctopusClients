using System;
using System.IO;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ICertificateConfigurationRepository : IGet<CertificateConfigurationResource>, IFindByName<CertificateConfigurationResource>
    {
        Task<CertificateConfigurationResource> GetOctopusCertificate();
        Task<Stream> GetPublicCertificate(CertificateConfigurationResource certificateConfiguration);
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

        public Task<Stream> GetPublicCertificate(CertificateConfigurationResource certificateConfiguration)
        {
            return Client.GetContent(certificateConfiguration.Links["PublicCer"]);
        }

        static string DetermineCollectionLinkName(IOctopusAsyncClient client)
        {
            if (client.RootDocument == null)
                throw new NullReferenceException("The client root document is null");

            // For backwards compatibility. 
            // In Octopus 3.11, what was Certificates was moved to CertificatesConfiguration, to make room for the certificates feature.
            // This allows pre-3.11 clients to still work.
            return client.HasLink("CertificateConfiguration")
                ? "CertificateConfiguration"
                : "Certificates";
        }
    }
}