using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ICertificateRepository : IGet<CertificateResource>, IFindByName<CertificateResource>
    {
        CertificateResource GetOctopusCertificate();
    }
    
    class CertificateRepository : BasicRepository<CertificateResource>, ICertificateRepository
    {
        public CertificateRepository(IOctopusClient client)
            : base(client, "Certificates")
        {
        }

        public CertificateResource GetOctopusCertificate()
        {
            return Get("certificate-global");
        }
    }
}