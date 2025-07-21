using System;
using System.Runtime.Serialization;

namespace Octopus.Client.Exceptions
{
    /// <summary>
    /// Base class for all exceptions thrown by the Octopus client.
    /// </summary>
    public abstract class OctopusException : Exception
    {
        readonly int httpStatusCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="OctopusException" /> class.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="message">The message.</param>
        protected OctopusException(int httpStatusCode, string message)
            : base(message)
        {
            this.httpStatusCode = httpStatusCode;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OctopusException" /> class.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        protected OctopusException(int httpStatusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            this.httpStatusCode = httpStatusCode;
        }

        protected OctopusException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Gets the HTTP status code.
        /// </summary>
        /// <value>
        /// The HTTP status code.
        /// </value>
        public int HttpStatusCode
        {
            get { return httpStatusCode; }
        }

        /// <summary>
        /// Gets additional help that the server may have provided regarding the error.
        /// </summary>
        public string HelpText { get; internal set; }
    }
}