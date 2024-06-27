﻿using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;
using Octopus.Client.Model.Endpoints;

namespace Octopus.Client.Model
{
    public abstract class MachineBasedResource : Resource, INamedResource, IHaveSlugResource, IHaveSpaceResource
    {
        [Trim]
        [Writeable]
        public string Name { get; set; }
        
        public string SpaceId { get; set; }

        /// <summary>
        /// Obsoleted as Server 3.4
        /// </summary>
        [Trim]
        [Writeable]
        public string Thumbprint { get; set; }

        /// <summary>
        /// Obsoleted as Server 3.4
        /// </summary>
        [Trim]
        [Writeable]
        public string Uri { get; set; }

        [Writeable]
        public bool IsDisabled { get; set; }

        [Writeable]
        public string MachinePolicyId { get; set; }

        /// <summary>
        /// Obsoleted as Server 3.4
        /// </summary>
        public MachineModelStatus Status { get; set; }

        public MachineModelHealthStatus HealthStatus { get; set; }
        public bool HasLatestCalamari { get; set; }
        public string StatusSummary { get; set; }

        public bool IsInProcess { get; set; }

        [Writeable]
        public EndpointResource Endpoint { get; set; }
        
        public string OperatingSystem { get; set; }
        public string ShellName { get; set; }
        public string ShellVersion { get; set; }
        public string Architecture { get; set; }
        
        public string Slug { get; set; }

        [Writeable]
        public bool SkipInitialHealthCheck { get; set; }
    }
}