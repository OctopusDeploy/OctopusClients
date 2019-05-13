using System;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Repositories
{
    public class SystemResourceIsIncompatibleWithSpaceScopedRepositoryException : Exception
    {
        public SystemResourceIsIncompatibleWithSpaceScopedRepositoryException(IResource resource)
            : base($"The system scoped resource {resource.Id} cannot be modified by a Space scoped repository. Try again using a repository scoped to System.")
        {
        }
    }
}