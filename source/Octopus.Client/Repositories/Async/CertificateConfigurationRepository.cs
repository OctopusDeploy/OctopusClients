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
        public CertificateConfigurationRepository(IOctopusAsyncRepository repository) : base(repository, null, async repo => await repository.HasLink("CertificateConfiguration").ConfigureAwait(false) ? "CertificateConfiguration" : "Certificates")
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
    }
}