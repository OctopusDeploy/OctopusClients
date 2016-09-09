using System;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Endpoints
{
    public class AzureWebAppEndpointResource : AgentlessEndpointResource
    {
        public override CommunicationStyle CommunicationStyle
        {
            get { return CommunicationStyle.AzureWebApp; }
        }

        [Trim]
        [Writeable]
        public string AccountId { get; set; }

        [Trim]
        [Writeable]
        public string WebSpaceName { get; set; }

        [Trim]
        [Writeable]
        public string WebAppName { get; set; }
    }
}