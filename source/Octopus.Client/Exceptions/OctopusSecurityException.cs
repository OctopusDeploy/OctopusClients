using System;

namespace Octopus.Client.Exceptions
{
    /// <summary>
    /// An exception thrown when the Octopus Server responds with HTTP 401 or 403, indicating that the current
    /// user's API key was not valid, their account is disabled, or they don't have permission to perform the
    /// specified action.
    /// </summary>
    public class OctopusSecurityException : OctopusException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OctopusSecurityException" /> class.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="message">The message.</param>
        public OctopusSecurityException(int httpStatusCode, string message) : base(httpStatusCode, message)
        {
        }
    }
}