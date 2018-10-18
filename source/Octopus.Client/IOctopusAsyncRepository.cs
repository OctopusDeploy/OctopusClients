using System.Threading.Tasks;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client
{
    /// <summary>
    /// A simplified interface to commonly-used parts of the API.
    /// Functionality not exposed by this interface can be accessed
    /// using <see cref="IOctopusAsyncRepository.Client" />.
    /// </summary>
    public interface IOctopusAsyncRepository: IOctopusSpaceAsyncRepository, IOctopusSystemAsyncRepository
    {
        /// <summary>
        /// The client over which the repository is run.
        /// </summary>
        IOctopusAsyncClient Client { get; }
        SpaceContext SpaceContext { get; }
        
        /// <summary>
        /// Determines whether the specified link exists.
        /// </summary>
        /// <param name="name">The name/key of the link.</param>
        /// <returns>
        /// <c>true</c> if the specified link is defined; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> HasLink(string name);

        /// <summary>
        /// Gets the link with the specified name.
        /// </summary>
        /// <param name="name">The name/key of the link.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">If the link is not defined.</exception>
        Task<string> Link(string name);
    }
}