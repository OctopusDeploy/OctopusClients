using System;
using System.ComponentModel.DataAnnotations;
using Octopus.Client.Extensibility.Attributes;

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
        public string ResourceGroupName { get; set; }

        [Trim]
        [Writeable]
        public string WebAppName { get; set; }

        [Trim]
        [Writeable]
        public string DefaultWorkerPoolId { get; set; }
    }
}