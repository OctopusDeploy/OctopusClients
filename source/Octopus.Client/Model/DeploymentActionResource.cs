using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Octopus.Client.Model
{
    public class DeploymentActionResource : Resource
    {
        public string Name { get; set; }
        public string ActionType { get; set; }
        public bool IsDisabled { get; set; }
        public string WorkerPoolId { get; set; }
        public string WorkerPoolVariable { get; set; }
        public bool CanBeUsedForProjectVersioning { get; set; }
        public string WorkerContainerImage { get; set; }
        public string WorkerContainerFeedId { get; set; }
        
        /// <summary>
        /// If true, this action cannot be skipped when deploying
        /// </summary>
        public bool IsRequired { get; set; }

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public ReferenceCollection Environments { get; } = new ReferenceCollection();

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public ReferenceCollection ExcludedEnvironments { get; } = new ReferenceCollection();

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public ReferenceCollection Channels { get; } = new ReferenceCollection();

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public ReferenceCollection TenantTags { get; } = new ReferenceCollection();

        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public IDictionary<string, PropertyValueResource> Properties { get; } = new Dictionary<string, PropertyValueResource>(StringComparer.OrdinalIgnoreCase);
        
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Reuse)]
        public PackageReferenceCollection Packages { get; } = new PackageReferenceCollection(); 

        public DeploymentActionResource ClearAllConditions()
        {
            Channels.Clear();
            Environments.Clear();
            ExcludedEnvironments.Clear();
            TenantTags.Clear();
            return this;
        }

        public DeploymentActionResource ForChannels(params ChannelResource[] channels)
        {
            Channels.ReplaceAll(channels.Select(c => c.Id));
            return this;
        }

        public DeploymentActionResource ForEnvironments(params EnvironmentResource[] environments)
        {
            Environments.ReplaceAll(environments.Select(e => e.Id));
            return this;
        }

        public DeploymentActionResource ForTenantTags(params TagResource[] tags)
        {
            TenantTags.ReplaceAll(tags.Select(t => t.CanonicalTagName));
            return this;
        }
    }
}