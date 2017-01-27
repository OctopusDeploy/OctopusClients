using System.IO;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ICertificateRepository : IGet<CertificateResource>, IFindByName<CertificateResource>, ICreate<CertificateResource>, IModify<CertificateResource>, IDelete<CertificateResource>
    {
        /// <summary>
        /// Exports the certificate data.
        /// </summary>
        /// <param name="certificate">The certificate to export.</param>
        /// <param name="format">The format of the exported certificate. If null, the certificate will be exported exactly as it was originally uploaded (including with original password).</param>
        /// <param name="password">Password for the exported file.  This value is only used if exporting to PCKS#12 or PEM formats.</param>
        /// <param name="includePrivateKey">Specifies whether the certificate private-key (if present) should be included in the exported file.  This value is only be used when exporting to PEM format.</param>
        /// <returns>The exported certificate data.</returns>
        Task<Stream> Export(CertificateResource certificate, CertificateFormat? format = null, string password = null, bool includePrivateKey = false);
    }

    class CertificateRepository : BasicRepository<CertificateResource>, ICertificateRepository
    {
        public CertificateRepository(IOctopusAsyncClient client)
            : base(client, "Certificates")
        {
        }

        public Task<Stream> Export(CertificateResource certificate, CertificateFormat? format = null, string password = null, bool includePrivateKey = false)
        {
            var pathParameters = format.HasValue ? new { format= format.Value, password = password, includePrivateKey = includePrivateKey} : null; 
            return Client.GetContent(certificate.Link("Export"), pathParameters);
        }
    }
}
