using System;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Repositories
{
    public class DefaultSpaceNotFoundException : Exception
    {
        public DefaultSpaceNotFoundException(IHaveSpaceResource spaceResource) : base($"Couldn't find a default space while trying to create or modify {((IResource)spaceResource).Id}, this could either mean that the default space has been disabled, or you do not have access to the default space.")
        {
        }
    }
}