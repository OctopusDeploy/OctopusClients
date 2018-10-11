using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;

namespace Octopus.Client
{
    /// <summary>
    /// A simplified interface to commonly-used parts of the API.
    /// Functionality not exposed by this interface can be accessed
    /// using <see cref="IOctopusAsyncRepository.Client" />.
    /// </summary>
    public interface IOctopusAsyncRepository : IAsyncSpaceRepository, IAsyncSystemRepository
    {
        /// <summary>
        /// The client over which the repository is run.
        /// </summary>
        IOctopusAsyncClient Client { get; }
    }
}