using Octopus.Client.Model;

namespace Octopus.Client
{
    /// <summary>
    /// A simplified interface to commonly-used parts of the API.
    /// Functionality not exposed by this interface can be accessed
    /// using <see cref="IOctopusCommonRepository.Client" />.
    /// </summary>
    public interface IOctopusRepository: IOctopusSpaceRepository, IOctopusSystemRepository
    {
        IOctopusSpaceRepository ForSpace(SpaceResource space);
        IOctopusSystemRepository ForSystem();
    }
}