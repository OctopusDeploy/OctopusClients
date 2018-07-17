using System;

namespace Octopus.Client.Exceptions
{
    public class InvalidSpacesLimitationException: Exception
    {
        public InvalidSpacesLimitationException(): base("The new spaceIds can only be a subset of the previously defined spaceIds")
        {
        }
    }
}