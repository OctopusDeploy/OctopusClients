using System;
using Octopus.Client.Extensibility;
using Octopus.Client.Model;

namespace Octopus.Client.Repositories
{
    public class ResourceSpaceDoesNotMatchRepositorySpaceException : Exception
    {
        public ResourceSpaceDoesNotMatchRepositorySpaceException(
            IHaveSpaceResource spaceResource, 
            SpaceResource repositorySpace) 
            : base($"The resource has a different space specified than the one specified by the repository scope. " +
                   $"Either change the {nameof(IHaveSpaceResource.SpaceId)} on the resource to {repositorySpace.Id}, " +
                   $"or use a repository that is scoped to {spaceResource.SpaceId}.")
        {
        }
    }
}