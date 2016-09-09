using System;

namespace Octopus.Client.Exceptions
{
    /// <summary>
    /// An exception thrown when the Octopus Server supports a version of the API that is incompatible with this class
    /// library.
    /// </summary>
    public class UnsupportedApiVersionException : OctopusException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedApiVersionException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnsupportedApiVersionException(string message) : base(200, message)
        {
        }
    }
}