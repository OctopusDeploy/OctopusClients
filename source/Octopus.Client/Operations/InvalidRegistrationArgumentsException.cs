using System;

namespace Octopus.Client.Operations
{
    public class InvalidRegistrationArgumentsException : ArgumentException
    {
        public InvalidRegistrationArgumentsException(string message)
            : base(message)
        { }
    }
}
