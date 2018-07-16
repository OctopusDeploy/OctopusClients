using System;

namespace Octopus.Client.Exceptions
{
    public class InvalidSpacesLimitationParametersException: Exception
    {
        public InvalidSpacesLimitationParametersException(): base("The new spaceIds can only be a subset of the previously defined spaceIds")
        {
        }
    }
}