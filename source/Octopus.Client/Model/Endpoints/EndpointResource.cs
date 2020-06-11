using System;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model.Endpoints
{
    public abstract class EndpointResource : Resource
    {
        public abstract CommunicationStyle CommunicationStyle { get; }
    }
}