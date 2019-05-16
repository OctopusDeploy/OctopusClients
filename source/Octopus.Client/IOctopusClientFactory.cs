using System;
using System.Threading.Tasks;

namespace Octopus.Client
{
    /// <summary>
    /// Creates instances of <see cref="IOctopusAsyncClient" />.
    /// </summary>
    public interface IOctopusClientFactory
    {
        /// <summary>
        /// Creates an appropriate <see cref="IOctopusClient" /> for the provided <see cref="OctopusServerEndpoint" />.
        /// </summary>
        /// <param name="serverEndpoint">The endpoint to create a client for.</param>
        /// <returns>The <see cref="IOctopusClient" /> instance.</returns>
        IOctopusClient CreateClient(OctopusServerEndpoint serverEndpoint);

        /// <summary>
        /// Creates an appropriate <see cref="IOctopusAsyncClient" /> for the provided <see cref="OctopusServerEndpoint" />.
        /// </summary>
        /// <param name="serverEndpoint">The endpoint to create a client for.</param>
        /// <param name="octopusClientOptions"></param>
        /// <returns>The <see cref="IOctopusAsyncClient" /> instance.</returns>
        Task<IOctopusAsyncClient> CreateAsyncClient(OctopusServerEndpoint serverEndpoint, OctopusClientOptions octopusClientOptions = null);
    }
}