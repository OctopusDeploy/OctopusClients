using System;

namespace Octopus.Client
{
    /// <summary>
    /// Creates instances of <see cref="IOctopusClient" />.
    /// </summary>
    public interface IOctopusClientFactory
    {
        /// <summary>
        /// Creates an appropriate <see cref="IOctopusClient" /> for the provided <see cref="OctopusServerEndpoint" />.
        /// </summary>
        /// <param name="serverEndpoint">The endpoint to create a client for.</param>
        /// <returns>The <see cref="IOctopusClient" /> instance.</returns>
        IOctopusClient CreateClient(OctopusServerEndpoint serverEndpoint);
    }
}