using System.Threading.Tasks;
using Octopus.Client.Repositories.Async;

namespace Octopus.Client
{
    /// <summary>
    /// A simplified interface to commonly-used parts of the API.
    /// Functionality not exposed by this interface can be accessed
    /// using <see cref="IOctopusCommonAsyncRepository.Client" />.
    /// </summary>
    public interface IOctopusAsyncRepository: IOctopusSpaceAsyncRepository, IOctopusSystemAsyncRepository
    {
    }
}