using System;
using System.IO;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client.Repositories
{
    public interface ICertificateRepository : IGet<CertificateResource>, IFindByName<CertificateResource>, ICreate<CertificateResource>, IModify<CertificateResource>, IDelete<CertificateResource>
    {
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="certificate"></param>
        /// <param name="format"></param>
        /// <param name="includePrivateKey"></param>
        /// <returns></returns>
        Stream Export(CertificateResource certificate, CertificateFormat? format = null, bool includePrivateKey = false);
        */
    }
    
    class CertificateRepository : BasicRepository<CertificateResource>, ICertificateRepository
    {
        public CertificateRepository(IOctopusClient client)
            : base(client, "Certificates")
        {
        }

        /*
        public Stream Export(CertificateResource certificate, CertificateFormat? format = null, bool includePrivateKey = false)
        {
            return Client.GetContent(certificate.Links("Export"));
        }
        */
    }
}