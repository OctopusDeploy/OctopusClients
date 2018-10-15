#if SYNC_CLIENT
namespace Octopus.Client
{
    /// <summary>
    /// A simplified interface to commonly-used parts of the API.
    /// Functionality not exposed by this interface can be accessed
    /// using <see cref="IOctopusRepository.Client" />.
    /// </summary>
    public interface IOctopusRepository: IOctopusSpaceRepository, IOctopusSystemRepository
    {
        /// <summary>
        /// The client over which the repository is run.
        /// </summary>
        IOctopusClient Client { get; }
        SpaceContext SpaceContext { get; }
        /// <summary>
        /// Determines whether the specified link exists.
        /// </summary>
        /// <param name="name">The name/key of the link.</param>
        /// <returns>
        /// <c>true</c> if the specified link is defined; otherwise, <c>false</c>.
        /// </returns>
        bool HasLink(string name);

        /// <summary>
        /// Gets the link with the specified name.
        /// </summary>
        /// <param name="name">The name/key of the link.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">If the link is not defined.</exception>
        string Link(string name);
    }
}
#endif