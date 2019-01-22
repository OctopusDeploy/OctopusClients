using System;

namespace Octopus.Client
{
    public class SpaceContextSwitchException : Exception
    {
        public SpaceContextSwitchException() : base(
            "Cannot switch to a custom space context when the repository has already been explicitly scoped to a context. " +
            "Use client.Repository to obtain a repository that has NOT been explicitly scoped to a context")
        {
        }
    }
}