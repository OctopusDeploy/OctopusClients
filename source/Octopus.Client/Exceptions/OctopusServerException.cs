using System;

namespace Octopus.Client.Exceptions
{
    /// <summary>
    /// An exception thrown when the Octopus Server responds with HTTP 500 or any other error, indicating that there was a
    /// problem processing
    /// the request.
    /// </summary>
    public class OctopusServerException : OctopusException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OctopusServerException" /> class.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="message">The message.</param>
        public OctopusServerException(int httpStatusCode, string message)
            : base(httpStatusCode, message)
        {
        }
    }
}