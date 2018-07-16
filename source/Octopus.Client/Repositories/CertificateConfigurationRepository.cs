using System;
using System.IO;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ICertificateConfigurationRepository : IGet<CertificateConfigurationResource>, IFindByName<CertificateConfigurationResource>
    {
        CertificateConfigurationResource GetOctopusCertificate();
        Stream GetPublicCertificate(CertificateConfigurationResource certificateConfiguration);
    }

    class CertificateConfigurationRepository : BasicRepository<CertificateConfigurationResource>, ICertificateConfigurationRepository
    {
        public CertificateConfigurationRepository(IOctopusClient client) : base(client, DetermineCollectionLinkName(client))
        {
        }

        public CertificateConfigurationResource GetOctopusCertificate()
        {
            return Get("certificate-global");
        }

        public Stream GetPublicCertificate(CertificateConfigurationResource certificateConfiguration)
        {
            return Client.GetContent(certificateConfiguration.Links["PublicCer"]);
        }

        static string DetermineCollectionLinkName(IOctopusClient client)
        {
            // For backwards compatibility. 
            // In Octopus 3.11, what was Certificates was moved to CertificatesConfiguration, to make room for the certificates feature.
            // This allows pre-3.11 clients to still work.
            // The null check is just for tests.
            return client.RootDocument == null || client.RootDocument.Links.ContainsKey("CertificateConfiguration")
                ? "CertificateConfiguration"
                : "Certificates";
        }
    }
}