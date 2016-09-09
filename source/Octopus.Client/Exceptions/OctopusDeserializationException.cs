using System;

namespace Octopus.Client.Exceptions
{
    public class OctopusDeserializationException : OctopusException
    {
        public OctopusDeserializationException(int httpStatusCode, string message) : base(httpStatusCode, message)
        {
        }

        public OctopusDeserializationException(int httpStatusCode, string message, Exception innerException) : base(httpStatusCode, message, innerException)
        {
        }
    }
}