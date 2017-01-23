using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ICertificateConfigurationRepository : IGet<CertificateConfigurationResource>, IFindByName<CertificateConfigurationResource>
    {
        Task<CertificateConfigurationResource> GetOctopusCertificate();
    }

    class CertificateConfigurationRepository : BasicRepository<CertificateResource>, ICertificateRepository
    {
        public CertificateConfigurationRepository(IOctopusAsyncClient client) : base(client, "CertificateConfiguration")
        {
        }

        public Task<CertificateResource> GetOctopusCertificate()
        {
            return Get("certificate-global");
        }
    }
}