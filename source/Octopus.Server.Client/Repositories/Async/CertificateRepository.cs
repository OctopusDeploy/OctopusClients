using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories.Async
{
    public interface ICertificateRepository : IResourceRepository, IGet<CertificateResource>, IFindByName<CertificateResource>, ICreate<CertificateResource>, IModify<CertificateResource>, IDelete<CertificateResource>
    {
        /// <summary>
        /// Exports the certificate data.
        /// </summary>
        /// <param name="certificate">The certificate to export.</param>
        /// <param name="format">The format of the exported certificate. If null, the certificate will be exported exactly as it was originally uploaded (including with original password).</param>
        /// <param name="password">Password for the exported file.  This value is only used if exporting to PCKS#12 or PEM formats.</param>
        /// <param name="includePrivateKey">Specifies whether the certificate private-key (if present) should be included in the exported file.  This value is only be used when exporting to PEM format.</param>
        /// <returns>The exported certificate data.</returns>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<Stream> Export(CertificateResource certificate, CertificateFormat? format = null, string password = null, bool includePrivateKey = false);
        Task<Stream> Export(CertificateResource certificate, CancellationToken cancellationToken, CertificateFormat? format = null, string password = null, bool includePrivateKey = false);

        /// <summary>
        /// Exports the certificate in PEM format
        /// </summary>
        /// <param name="certificate">The certificate to export.</param>
        /// <param name="includePrivateKey">Specifies whether the certificate private-key (if present) should be included in the exported file.</param>
        /// <param name="pemOptions">Options specifying which certificates should be included when chain certificates are present</param>
        /// <returns>The exported certificate in PEM format</returns>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<Stream> ExportAsPem(CertificateResource certificate, bool includePrivateKey = false, CertificateExportPemOptions pemOptions = CertificateExportPemOptions.PrimaryOnly);
        Task<Stream> ExportAsPem(CertificateResource certificate, CancellationToken cancellationToken, bool includePrivateKey = false, CertificateExportPemOptions pemOptions = CertificateExportPemOptions.PrimaryOnly);

        /// <summary>
        /// Replace with a new certificate.
        /// The certificate is replaced "in-place"; it will retain the same ID and other user properties (Name, Notes, Environments, etc...).
        /// A backup will be made of the replaced certificate; it will have a new ID and will be archived.
        /// </summary>
        /// <param name="certificate">The certificate to be replaced</param>
        /// <param name="certificateData">The new base64-encoded certificate-data</param>
        /// <param name="password">The new password</param>
        /// <returns>The replaced certificate. The ReplacedBy property will contain the ID of the new certificate (which will be the previous ID of the replaced certificate).</returns>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<CertificateResource> Replace(CertificateResource certificate, string certificateData, string password);
        Task<CertificateResource> Replace(CertificateResource certificate, string certificateData, string password, CancellationToken cancellationToken);

        /// <summary>
        /// Archive a certificate.
        /// Archiving makes a certificate unavailable for selection as the value of a variable.
        /// </summary>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task Archive(CertificateResource certificate);
        Task Archive(CertificateResource certificate, CancellationToken cancellationToken);

        /// <summary>
        /// Unarchive a certificate. This makes the certificate again available for selection as the value of a variable.
        /// </summary>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task UnArchive(CertificateResource certificate);
        Task UnArchive(CertificateResource certificate, CancellationToken cancellationToken);

        // For backwards compatibility.
        // The CertificateRepository was renamed to CertificateConfigurationRepository when the Certificates feature was
        // implemented. This method avoids breaking existing scripts and code.
        /// <summary>
        /// Returns details of the certificate used by Octopus for communications.
        /// </summary>
        [Obsolete("Please use the overload with cancellation token instead.", false)]
        Task<CertificateConfigurationResource> GetOctopusCertificate();
        Task<CertificateConfigurationResource> GetOctopusCertificate(CancellationToken cancellationToken);
    }

    class CertificateRepository : BasicRepository<CertificateResource>, ICertificateRepository
    {
        public CertificateRepository(IOctopusAsyncRepository repository)
            : base(repository, "Certificates")
        {
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<Stream> Export(CertificateResource certificate, CertificateFormat? format = null, string password = null, bool includePrivateKey = false)
            => Export(certificate, CancellationToken.None, format, password, includePrivateKey);

        public Task<Stream> Export(CertificateResource certificate, CancellationToken cancellationToken, CertificateFormat? format = null, string password = null, bool includePrivateKey = false)
        {
            var pathParameters = format.HasValue ? new { format = format.Value, password = password, includePrivateKey = includePrivateKey } : null;
            return Client.GetContent(certificate.Link("Export"), pathParameters, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<Stream> ExportAsPem(CertificateResource certificate, bool includePrivateKey = false,
            CertificateExportPemOptions pemOptions = CertificateExportPemOptions.PrimaryOnly)
            => ExportAsPem(certificate, CancellationToken.None, includePrivateKey, pemOptions);

        public Task<Stream> ExportAsPem(CertificateResource certificate, CancellationToken cancellationToken, bool includePrivateKey = false,
            CertificateExportPemOptions pemOptions = CertificateExportPemOptions.PrimaryOnly)
        {
            var parameters = new { format = CertificateFormat.Pem, includePrivateKey, pemOptions };
            return Client.GetContent(certificate.Link("Export"), parameters, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<CertificateResource> Replace(CertificateResource certificate, string certificateData, string password)
            => Replace(certificate, certificateData, password, CancellationToken.None);

        public Task<CertificateResource> Replace(CertificateResource certificate, string certificateData, string password, CancellationToken cancellationToken)
        {
            return Client.Post<object, CertificateResource>(certificate.Link("Replace"), new { certificateData = certificateData, password = password }, cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task Archive(CertificateResource certificate)
            => Archive(certificate, CancellationToken.None);

        public Task Archive(CertificateResource certificate, CancellationToken cancellationToken)
        {
            return Client.Post(certificate.Link("Archive"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task UnArchive(CertificateResource certificate)
            => UnArchive(certificate, CancellationToken.None);

        public Task UnArchive(CertificateResource certificate, CancellationToken cancellationToken)
        {
            return Client.Post(certificate.Link("Unarchive"), cancellationToken);
        }

        [Obsolete("Please use the overload with cancellation token instead.", false)]
        public Task<CertificateConfigurationResource> GetOctopusCertificate()
            => GetOctopusCertificate(CancellationToken.None);

        public Task<CertificateConfigurationResource> GetOctopusCertificate(CancellationToken cancellationToken)
        {
            return Repository.CertificateConfiguration.GetOctopusCertificate(cancellationToken);
        }
    }
}
