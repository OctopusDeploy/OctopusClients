using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Octopus.Client
{
    /// <summary>
    /// Creates instances of the <see cref="IOctopusAsyncClient" />.
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
            var  requestingTool = DetermineRequestingTool();
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
            var requestingTool = DetermineRequestingTool();
            return OctopusAsyncClient.Create(serverEndpoint, octopusClientOptions, requestingTool);
        }

        private string DetermineRequestingTool()
        {
            var launchAssembly = Assembly.GetEntryAssembly();
                
            if (launchAssembly != null && launchAssembly.GetTypes().Any(x => x.FullName == "Octo.Program" || x.FullName == "Octopus.DotNet.Cli.Program"))
            {
                var octoExtensionVersion = Environment.GetEnvironmentVariable("OCTOEXTENSION");
                if (!string.IsNullOrWhiteSpace(octoExtensionVersion))
                    return $"octo plugin/{octoExtensionVersion}";

                return "octo";
            }

            return null;
        }
    }
}