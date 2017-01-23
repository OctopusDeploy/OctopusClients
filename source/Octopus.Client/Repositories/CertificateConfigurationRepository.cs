using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ICertificateConfigurationRepository : IGet<CertificateConfigurationResource>, IFindByName<CertificateConfigurationResource>
    {
        CertificateConfigurationResource GetOctopusCertificate();
    }

    class CertificateConfigurationRepository : BasicRepository<CertificateConfigurationResource>, ICertificateConfigurationRepository
    {
        public CertificateConfigurationRepository(IOctopusClient client) : base(client, "CertificateConfiguration")
        {
        }

        public CertificateConfigurationResource GetOctopusCertificate()
        {
            return Get("certificate-global");
        }
    }
}