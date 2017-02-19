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

        /// <summary>
        /// Replace with a new certificate.  
        /// The certificate is replaced "in-place"; it will retain the same ID and other user properties (Name, Notes, Environments, etc...).
        /// A backup will be made of the replaced certificate; it will have a new ID and will be archived.  
        /// </summary>
        /// <param name="certificate">The certificate to be replaced</param>
        /// <param name="certificateData">The new base64-encoded certificate-data</param>
        /// <param name="password">The new password</param>
        /// <returns>The replaced certificate. The ReplacedBy property will contain the ID of the new certificate (which will be the previous ID of the replaced certificate).</returns>
        Task<CertificateResource> Replace(CertificateResource certificate, string certificateData, string password);

        /// <summary>
        /// Archive a certificate. 
        /// Archiving makes a certificate unavailable for selection as the value of a variable. 
        /// </summary>
        Task Archive(CertificateResource certificate);

        /// <summary>
        /// Unarchive a certificate. This makes the certificate again available for selection as the value of a variable.
        /// </summary>
        Task UnArchive(CertificateResource certificate);
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

        public Task<CertificateResource> Replace(CertificateResource certificate, string certificateData, string password)
        {
            return Client.Post<object, CertificateResource>(certificate.Link("Replace"), new {certificateData = certificateData, password = password});
        }

        public Task Archive(CertificateResource certificate)
        {
            return Client.Post(certificate.Link("Archive"));
        }

        public Task UnArchive(CertificateResource certificate)
        {
            return Client.Post(certificate.Link("Unarchive"));
        }
    }
}
