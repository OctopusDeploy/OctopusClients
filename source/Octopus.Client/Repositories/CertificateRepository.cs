using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ICertificateRepository : IGet<CertificateResource>, IFindByName<CertificateResource>
    {
    }
    
    class CertificateRepository : BasicRepository<CertificateResource>, ICertificateRepository
    {
        public CertificateRepository(IOctopusClient client)
            : base(client, "Certificates")
        {
        }
    }
}