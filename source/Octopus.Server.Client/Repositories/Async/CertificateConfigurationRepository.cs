using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ICertificateConfigurationRepository : IGet<CertificateConfigurationResource>, IFindByName<CertificateConfigurationResource>
    {
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<CertificateConfigurationResource> GetOctopusCertificate();
        Task<CertificateConfigurationResource> GetOctopusCertificate(CancellationToken cancellationToken);
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<Stream> GetPublicCertificate(CertificateConfigurationResource certificateConfiguration);
        Task<Stream> GetPublicCertificate(CertificateConfigurationResource certificateConfiguration, CancellationToken cancellationToken);
    }

    class CertificateConfigurationRepository : BasicRepository<CertificateConfigurationResource>, ICertificateConfigurationRepository
    {
        public CertificateConfigurationRepository(IOctopusAsyncRepository repository) : base(repository, null, async repo => await repository.HasLink("CertificateConfiguration").ConfigureAwait(false) ? "CertificateConfiguration" : "Certificates")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<CertificateConfigurationResource> GetOctopusCertificate()
        {
            return GetOctopusCertificate(CancellationToken.None);
        }

        public Task<CertificateConfigurationResource> GetOctopusCertificate(CancellationToken cancellationToken)
        {
            return Get("certificate-global", cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<Stream> GetPublicCertificate(CertificateConfigurationResource certificateConfiguration)
        {
            return GetPublicCertificate(certificateConfiguration, CancellationToken.None);
        }

        public Task<Stream> GetPublicCertificate(CertificateConfigurationResource certificateConfiguration, CancellationToken cancellationToken)
        {
            return Client.GetContent(certificateConfiguration.Links["PublicCer"], cancellationToken);
        }
    }
}
