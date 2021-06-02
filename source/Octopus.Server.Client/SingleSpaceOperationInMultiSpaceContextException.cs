using System;

namespace Octopus.Client
{
    public class SingleSpaceOperationInMultiSpaceContextException : Exception
    {
        public SingleSpaceOperationInMultiSpaceContextException() : base("Attempted to perform a space scoped operation against a single space while in a multiple space context. " +
                                                                         "Ensure you are in a single space context first by using client.ForSpace(string spaceId)")
        {
        }
    }
}