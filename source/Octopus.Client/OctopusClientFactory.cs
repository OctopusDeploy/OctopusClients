using System;
using System.Threading.Tasks;

namespace Octopus.Client
{
    /// <summary>
    /// Creates instances of the <see cref="IOctopusAsyncClient" />.
    /// </summary>
    public class OctopusClientFactory : IOctopusClientFactory
    {
        private static string requestingTool;

        public static void SetRequestingTool(string toolName)
        {
            requestingTool = toolName;
        }

        /// <summary>
        /// Creates an instance of the client.
        /// </summary>
        /// <param name="serverEndpoint">The server endpoint.</param>
        /// <returns>The <see cref="IOctopusClient" /> instance.</returns>
        public IOctopusClient CreateClient(OctopusServerEndpoint serverEndpoint)
        {
            return new OctopusClient(serverEndpoint, requestingTool);
        }

        /// <summary>
        /// Creates an instance of the client.
        /// </summary>
        /// <param name="serverEndpoint">The server endpoint.</param>
        /// <param name="octopusClientOptions"></param>
        /// <returns>The <see cref="IOctopusAsyncClient" /> instance.</returns>
        public Task<IOctopusAsyncClient> CreateAsyncClient(OctopusServerEndpoint serverEndpoint, OctopusClientOptions octopusClientOptions = null)
        {
            return OctopusAsyncClient.Create(serverEndpoint, octopusClientOptions, requestingTool);
        }
    }
}