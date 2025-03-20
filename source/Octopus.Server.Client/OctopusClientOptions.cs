using System;
using System.Net;
using System.Security.Authentication;
using Octopus.Client.Model;

namespace Octopus.Client
{
    /// <summary>
    /// Options used to change the behaviour of <see cref="OctopusAsyncClient" />
    /// </summary>
    public class OctopusClientOptions
    {
        public OctopusClientOptions()
        {
            Timeout = TimeSpan.FromMilliseconds(ApiConstants.DefaultClientRequestTimeout);
#if HTTP_CLIENT_SUPPORTS_SSL_OPTIONS
            SslProtocols = SslProtocols.Tls
                           | SslProtocols.Tls11
                           | SslProtocols.Tls12;
#endif
        }
#if HTTP_CLIENT_SUPPORTS_SSL_OPTIONS
        /// <summary>
        /// The allowed SSL Protocols
        /// </summary>
        public SslProtocols SslProtocols { get; set; }

        /// <summary>
        /// If true, SSL certificate errors are ignored
        /// </summary>
        public bool IgnoreSslErrors { get; set; }

#endif
        public TimeSpan Timeout { get; set; }
        public string Proxy { get; set; }
        public string ProxyUsername { get; set; }
        public string ProxyPassword { get; set; }
        
        /// <summary>
        /// Whether or not the default proxy can be used if the proxy is not set.
        /// </summary>
        public bool AllowDefaultProxy { get; set; } = true;

        /// <summary>
        ///     Provides a way for Octopus.Server.Client to scan for all types implementing IHttpRequestRouteFor or
        ///     IHttpCommandRouteFor.
        /// </summary>
        /// <remarks>
        ///     Will be hit every time a new type of command or request payload is sent to allow for dynamic loading of assemblies
        ///     into the appdomain, so a reasonably-well-filtered collection is recommended.
        /// </remarks>
        public Func<Type[]> ScanForHttpRouteTypes { get; set; } = AppDomainScanner.ScanForAllTypes;

        /// <summary>
        /// Maximum number of simultaneous requests to make to the server
        /// </summary>
        public int MaxSimultaneousRequests = int.MaxValue;
    }
}