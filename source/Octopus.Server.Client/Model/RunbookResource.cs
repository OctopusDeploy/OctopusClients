﻿using Newtonsoft.Json;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    public class RunbookResource : Resource, INamedResource, IHaveSpaceResource, IHaveSlugResource
    {
        [Writeable]
        [Trim]
        public string Name { get; set; }
        
        [Writeable]
        [Trim]
        public string Slug { get; set; }

        [Writeable]
        [Trim]
        public string Description { get; set; }

        [Writeable]
        public string RunbookProcessId { get; set; }

        [Writeable]
        public string PublishedRunbookSnapshotId { get; set; }

        [Writeable]
        public string ProjectId { get; set; }

        public string SpaceId { get; set; }

        [Writeable]
        public TenantedDeploymentMode MultiTenancyMode { get; set; }

        [Writeable]
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public DeploymentConnectivityPolicy ConnectivityPolicy { get; set; } = new DeploymentConnectivityPolicy() { AllowDeploymentsToNoTargets = true};

        [Writeable]
        public RunbookEnvironmentScope EnvironmentScope { get; set; }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public ReferenceCollection Environments { get; } = new ReferenceCollection();

        [Writeable]
        public GuidedFailureMode DefaultGuidedFailureMode { get; set; }

        [Writeable]
        public RunbookRetentionPeriod RunRetentionPolicy { get; set; }
    }
}
