using System;

namespace Octopus.Client.Exceptions
{
    public class SpaceContextExtensionException : Exception
    {
        public SpaceContextExtensionException(string msg) : base(msg)
        {
        }
    }
}