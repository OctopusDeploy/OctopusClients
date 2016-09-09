using System;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public interface ICertificateRepository : IGet<CertificateResource>, IFindByName<CertificateResource>
    {
        CertificateResource GetOctopusCertificate();
    }
}