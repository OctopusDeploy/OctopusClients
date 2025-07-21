using System;

namespace Octopus.Client.Exceptions
{
    /// <summary>
    /// An exception thrown when the Octopus Server responds with HTTP 404, such as when the specified
    /// resource does not exist on the server.
    /// </summary>
    public class OctopusResourceNotFoundException : OctopusException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OctopusResourceNotFoundException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public OctopusResourceNotFoundException(string message)
            : base((int)System.Net.HttpStatusCode.NotFound, message)
        {
        }
    }
}