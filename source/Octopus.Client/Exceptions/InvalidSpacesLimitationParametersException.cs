using System;

namespace Octopus.Client.Exceptions
{
    public class InvalidSpacesLimitationParametersException : OctopusException
    {
        public InvalidSpacesLimitationParametersException(): base((int)System.Net.HttpStatusCode.Conflict, "The new spaceIds should be a subset of the previously defined spaceIds")
        {
        }
    }
}