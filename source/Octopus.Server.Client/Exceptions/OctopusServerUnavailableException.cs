using System;

namespace Octopus.Client.Exceptions
{
    /// <summary>
    /// An exception thrown when the Octopus Server, or an intermediary in front of it, responds with HTTP 502, 503 or
    /// 504. The instance could not serve the request at all, rather than the request itself being at fault, so the same
    /// request may succeed once the instance is available again.
    /// </summary>
    public class OctopusServerUnavailableException : OctopusServerException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OctopusServerUnavailableException" /> class.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="message">The message.</param>
        public OctopusServerUnavailableException(int httpStatusCode, string message)
            : base(httpStatusCode, message)
        {
        }
    }
}
