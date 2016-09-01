using System;
using System.Net;
using System.Net.Http;

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
        event Action<HttpRequestMessage> BeforeSendingHttpRequest;
    }
}