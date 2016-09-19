using System;
using System.Threading.Tasks;

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
        /// <param name="octopusClientOptions"></param>
        /// <returns>The <see cref="IOctopusClient" /> instance.</returns>
        public Task<IOctopusClient> CreateClient(OctopusServerEndpoint serverEndpoint, OctopusClientOptions octopusClientOptions = null)
        {
            return OctopusClient.Create(serverEndpoint, octopusClientOptions);
        }
    }
}