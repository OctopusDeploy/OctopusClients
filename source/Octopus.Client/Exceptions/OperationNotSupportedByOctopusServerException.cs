namespace Octopus.Client.Exceptions
{
    /// <summary>
    /// An exception thrown when the Octopus Server does not support a given operation
    /// </summary>
    public class OperationNotSupportedByOctopusServerException : OctopusException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationNotSupportedByOctopusServerException" /> class.
        /// </summary>
        /// <param name="message">The detailed exception message.</param>
        /// <param name="requiresVersion">The version required for the specified operation.</param>
        public OperationNotSupportedByOctopusServerException(string message, string requiresVersion)
            : base((int)System.Net.HttpStatusCode.NotImplemented, message)
        {
            RequiredOctopusVersion = requiresVersion;
        }

        public string RequiredOctopusVersion { get; }
    }
}
