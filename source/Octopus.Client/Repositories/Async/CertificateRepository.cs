using System;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ICertificateRepository : IGet<CertificateResource>, IFindByName<CertificateResource>
    {
        Task<CertificateResource> GetOctopusCertificate();
    }

    class CertificateRepository : BasicRepository<CertificateResource>, ICertificateRepository
    {
        public CertificateRepository(IOctopusAsyncClient client)
            : base(client, "Certificates")
        {
        }

        public Task<CertificateResource> GetOctopusCertificate()
        {
            return Get("certificate-global");
        }
    }
}
