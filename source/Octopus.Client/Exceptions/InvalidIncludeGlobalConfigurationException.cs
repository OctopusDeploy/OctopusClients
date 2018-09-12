using System;

namespace Octopus.Client.Exceptions
{
    public class InvalidIncludeSystemConfigurationException : Exception
    {
        public InvalidIncludeSystemConfigurationException(): base("Cannot include system resources when system resources have already been excluded")
        {
        }
    }
}