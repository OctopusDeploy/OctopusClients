using System.Threading.Tasks;
using Octopus.Client.Exceptions;
using Octopus.Client.Model;
using Octopus.Client.Repositories.Async;

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
        IEventRepository Events { get; }        
        ITaskRepository Tasks { get; }
        ITeamsRepository Teams { get; }
        /// <summary>
        /// Gets a document that identifies the Octopus server (from /api) and provides links to the resources available on the
        /// server. Instead of hardcoding paths,
        /// clients should use these link properties to traverse the resources on the server. This document is lazily loaded so
        /// that it is only requested once for
        /// the current <see cref="IOctopusAsyncClient" />.
        /// </summary>
        /// <exception cref="OctopusSecurityException">
        /// HTTP 401 or 403: Thrown when the current user's API key was not valid, their
        /// account is disabled, or they don't have permission to perform the specified action.
        /// </exception>
        /// <exception cref="OctopusServerException">
        /// If any other error is successfully returned from the server (e.g., a 500
        /// server error).
        /// </exception>
        /// <exception cref="OctopusValidationException">HTTP 400: If there was a problem with the request provided by the user.</exception>
        /// <exception cref="OctopusResourceNotFoundException">HTTP 404: If the specified resource does not exist on the server.</exception>
        RootResource RootDocument { get; }
        /// <summary>
        /// Requests a fresh root document from the Octopus Server which can be useful if the API surface has changed. This can occur when enabling/disabling features, or changing license.
        /// </summary>
        /// <returns>A fresh copy of the root document.</returns>
        Task<RootResource> RefreshRootDocument();
    }
}