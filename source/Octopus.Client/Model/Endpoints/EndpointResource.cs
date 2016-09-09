using System;

namespace Octopus.Client.Model.Endpoints
{
    public abstract class EndpointResource : Resource
    {
        public abstract CommunicationStyle CommunicationStyle { get; }
    }
}