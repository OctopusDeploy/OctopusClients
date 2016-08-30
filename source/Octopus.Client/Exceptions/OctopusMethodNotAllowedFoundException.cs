using System;

namespace Octopus.Client.Exceptions
{
    /// <summary>
    /// An exception thrown when the Octopus Server responds with HTTP 405, which indicates that the
    /// HTTP method (GET, POST, PUT, DELETE) is not supported on the specified resource.
    /// </summary>
    public class OctopusMethodNotAllowedFoundException : OctopusException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OctopusResourceNotFoundException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public OctopusMethodNotAllowedFoundException(string message)
            : base((int)System.Net.HttpStatusCode.MethodNotAllowed, message)
        {
        }
    }
}