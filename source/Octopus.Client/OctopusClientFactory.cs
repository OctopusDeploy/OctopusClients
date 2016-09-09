using System;

namespace Octopus.Client
{
    /// <summary>
    /// Creates instances of the <see cref="IOctopusClient" />.
    /// </summary>
    public class OctopusClientFactory : IOctopusClientFactory
    {
        /// <summary>
        /// Creates an instance of the client.
        /// </summary>
        /// <param name="serverEndpoint">The server endpoint.</param>
        /// <returns>The <see cref="IOctopusClient" /> instance.</returns>
        public IOctopusClient CreateClient(OctopusServerEndpoint serverEndpoint)
        {
            return new OctopusClient(serverEndpoint);
        }
    }
}