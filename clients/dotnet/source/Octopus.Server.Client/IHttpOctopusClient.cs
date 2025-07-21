using System;
using System.Net;

namespace Octopus.Client
{
    /// <summary>
    /// Implemented by an <see cref="IOctopusClient" /> that uses HTTP to communicate.
    /// </summary>
    public interface IHttpOctopusClient : IOctopusClient
    {
        /// <summary>
        /// Occurs when a request is about to be sent.
        /// </summary>
        event Action<WebRequest> BeforeSendingHttpRequest;

        /// <summary>
        /// Occurs when a response has been received.
        /// </summary>
        event Action<WebResponse> AfterReceivingHttpResponse;
    }
}
