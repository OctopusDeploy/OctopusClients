﻿using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public class CloudRegionEndpointResource : EndpointResource
    {
        public override CommunicationStyle CommunicationStyle => CommunicationStyle.None;

        [Trim]
        [Writeable]
        public string DefaultWorkerPoolId { get; set; }
    }
}