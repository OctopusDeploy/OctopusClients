using System;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Repositories
{
    internal class DefaultSpaceNotFoundException : Exception
    {
        public DefaultSpaceNotFoundException(IHaveSpaceResource spaceResource) : base($"")
        {
            throw new NotImplementedException();
        }
    }
}