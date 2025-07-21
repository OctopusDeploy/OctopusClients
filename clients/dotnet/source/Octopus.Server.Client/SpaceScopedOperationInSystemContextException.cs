using System;

namespace Octopus.Client
{
    public class SpaceScopedOperationInSystemContextException : Exception
    {
        public SpaceScopedOperationInSystemContextException() 
            : base("Attempted to perform a space scoped operation in a system context. Ensure you are in a space context first by using client.ForSpace(string spaceId)")
        {
        }
    }
}