using System;
using System.Runtime.Serialization;
using Octopus.Client.Exceptions;

namespace Octopus.Client.HttpRouting
{
    [Serializable]
    public class PayloadRoutingException : OctopusException
    {
        public PayloadRoutingException(string message) : base(400, message)
        {
        }

        public PayloadRoutingException(string message, Exception inner) : base(400, message, inner)
        {
        }

        protected PayloadRoutingException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}