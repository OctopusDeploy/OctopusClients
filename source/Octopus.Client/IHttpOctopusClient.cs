#if SYNC_CLIENT
using System;
using System.Net;

namespace Octopus.Client
{
    /// <summary>
    /// Implemented by an <see cref="IOctopusClient" /> that uses HTTP to communicate.
    /// </summary>
    // [Obsolete("Use IOctopusAsyncClient instead")]
    public interface IHttpOctopusClient : IOctopusClient
    {
        /// <summary>
        /// Occurs when a request is about to be sent.
        /// </summary>
        event Action<WebRequest> BeforeSendingHttpRequest;
    }
}
#endif