using System;

namespace Octopus.Client.Exceptions
{
    public class InvalidIncludeGlobalConfigurationException : Exception
    {
        public InvalidIncludeGlobalConfigurationException(): base("Cannot include global resources when global resources have already been excluded")
        {
        }
    }
}