using System;
using Octopus.Client.Extensibility.Attributes;
using Newtonsoft.Json;
using Octopus.Client.Extensibility;

namespace Octopus.Client.Model
{
    public class MachinePolicyResource : Resource, INamedResource, IHaveSpaceResource
    {
        public MachinePolicyResource()
        {
            MachineHealthCheckPolicy = new MachineHealthCheckPolicy();
            MachineConnectivityPolicy = new MachineConnectivityPolicy();
            MachineCleanupPolicy = new MachineCleanupPolicy();
            MachineUpdatePolicy = new MachineUpdatePolicy();
            MachineRpcCallRetryPolicy = new MachineRpcCallRetryPolicy();
        }

        [Writeable]
        [JsonProperty(Order = 2)]
        public string Name { get; set; }

        [Writeable]
        [JsonProperty(Order = 20)]
        public string Description { get; set; }

        [Writeable]
        [JsonProperty(Order = 25)]
        public bool IsDefault { get; set; }

        [Writeable]
        [JsonProperty(Order = 30, ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public MachineHealthCheckPolicy MachineHealthCheckPolicy { get; set; }

        [Writeable]
        [JsonProperty(Order = 40, ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public MachineConnectivityPolicy MachineConnectivityPolicy { get; set; }

        [Writeable]
        [JsonProperty(Order = 45, ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public MachineCleanupPolicy MachineCleanupPolicy { get; set; }

        [Writeable]
        [JsonProperty(Order=50, ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public MachineUpdatePolicy MachineUpdatePolicy { get; set; }

        [Writeable]
        [JsonProperty(Order=55, ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public MachineRpcCallRetryPolicy MachineRpcCallRetryPolicy { get; set; }
        
        [Writeable]
        [JsonProperty(Order = 60)]
        public TimeSpan PollingRequestQueueTimeout { get; set; }

        [Writeable]
        [JsonProperty(Order = 61)]
        [Obsolete("Not used since 2024.1.8781")]
        public TimeSpan PollingRequestMaximumMessageProcessingTimeout { get; set; }

        [Writeable]
        [JsonProperty(Order = 62)]
        public TimeSpan ConnectionRetrySleepInterval { get; set; }

        [Writeable]
        [JsonProperty(Order = 63)]
        public int ConnectionRetryCountLimit { get; set; }

        [Writeable]
        [JsonProperty(Order = 64)]
        public TimeSpan ConnectionRetryTimeLimit { get; set; }

        [Writeable]
        [JsonProperty(Order = 65)]
        public TimeSpan ConnectionConnectTimeout { get; set; }

        public string SpaceId { get; set; }
    }
}
