using System;

namespace Octopus.Client.Exceptions
{
    public class InvalidSpacesLimitationParametersException: Exception
    {
        public InvalidSpacesLimitationParametersException(): base("The new spaceIds can only be a subset of the previously defined spaceIds and includeGlobal should be the same as the previous value")
        {
        }
    }
}